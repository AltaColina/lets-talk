using Ardalis.Specification;
using Humanizer;
using LetsTalk.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LetsTalk.Services;
internal sealed class MongoRepository<T> : IRepository<T> where T : class, IEntity<string>
{
    private readonly IMongoCollection<T> _collection;

    public string CollectionName { get; } = typeof(T).Name.Pluralize().Camelize();

    public MongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(CollectionName);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        return entities;
    }

    public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        return await cursor.AnyAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return await cursor.AnyAsync(cancellationToken: cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        return (int)count;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return (int)count;
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(e => e.Id == entity.Id, cancellationToken: cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var ids = entities.Select(e => e.Id).ToList();
        await _collection.DeleteManyAsync(e => ids.Contains(e.Id), cancellationToken: cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        var result = await cursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return specification.Selector!.Compile().Invoke(result);
    }

    public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken) where TId : notnull
    {
        var cursor = await _collection.FindAsync(e => e.Id.Equals(id), cancellationToken: cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    [Obsolete]
    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    [Obsolete]
    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        var selector = specification.Selector!.Compile();
        return cursor.ToEnumerable(cancellationToken).Select(selector).ToList();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => throw new NotSupportedException();

    public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.WhereExpressions.ToExpression(), cancellationToken: cancellationToken);
        var result = await cursor.SingleOrDefaultAsync(cancellationToken: cancellationToken);
        return specification.Selector!.Compile().Invoke(result);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.BulkWriteAsync(entities.Select(e => new ReplaceOneModel<T>(Builders<T>.Filter.Eq(f => f.Id, e.Id), e)), cancellationToken: cancellationToken);
    }
}

internal static class SpecificationExtensions
{
    public static Expression<Func<T, bool>> ToExpression<T>(this IEnumerable<WhereExpressionInfo<T>> whereExpressions)
    {
        if (whereExpressions == null)
            throw new ArgumentNullException(nameof(whereExpressions));
        if (!whereExpressions.Any())
            return t => true;
        if (whereExpressions.Take(2).Count() == 1)
            return whereExpressions.Single().Filter;
        var expressions = whereExpressions.Select(e => e.Filter);
        var delegateType = typeof(Func<,>).GetGenericTypeDefinition().MakeGenericType(new[] { typeof(T), typeof(bool) });
        var combined = expressions.Cast<Expression>().Aggregate((p, c) => Expression.AndAlso(p, c));
        return (Expression<Func<T, bool>>)Expression.Lambda(delegateType, combined);
    }
}