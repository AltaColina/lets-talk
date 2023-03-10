using Ardalis.Specification;
using Humanizer;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LetsTalk.Repositories;
internal abstract class MongoRepository<T> : IRepository<T> where T : Entity
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
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.AnyAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return await cursor.AnyAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(specification.ToFilterDefinition(), specification.ToCountOptions(), cancellationToken);
        return (int)count;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return (int)count;
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteManyAsync(Builders<T>.Filter.In(e => e.Id, entities.Select(e => e.Id)), cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<T?> FindOneAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
    {
        var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken) where TId : notnull => FindOneAsync(e => e.Id.Equals(id), cancellationToken);

    public Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken) => FindOneAsync(e => e.Id == id, cancellationToken);

    public Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => FindOneAsync(e => e.Name == name, cancellationToken);

    [Obsolete]
    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken) => throw new NotSupportedException();

    [Obsolete]
    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => throw new NotSupportedException();

    public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var k = await _collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);
        Console.WriteLine(k);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.BulkWriteAsync(entities.Select(e => new ReplaceOneModel<T>(Builders<T>.Filter.Eq(f => f.Id, e.Id), e)), cancellationToken: cancellationToken);
    }

}

internal static class SpecificationExtensions
{
    public static FilterDefinition<T> ToFilterDefinition<T>(this ISpecification<T> specification)
    {
        if (!specification.WhereExpressions.Any())
            return Builders<T>.Filter.Empty;
        if (specification.WhereExpressions.Take(2).Count() == 1)
            return Builders<T>.Filter.Where(specification.WhereExpressions.Single().Filter);
        return Builders<T>.Filter.And(specification.WhereExpressions.Select(e => Builders<T>.Filter.Where(e.Filter)));
    }

    public static CountOptions? ToCountOptions<T>(this ISpecification<T> specification)
    {
        if (specification.Take is null && specification.Skip is null)
            return null;
        return new CountOptions
        {
            Limit = specification.Take,
            Skip = specification.Skip
        };
    }

    public static FindOptions<T>? ToFindOptions<T>(this ISpecification<T> specification)
    {
        if (specification.Take is null && specification.Skip is null && !specification.OrderExpressions.Any())
            return null;
        return new FindOptions<T>
        {
            Limit = specification.Take,
            Skip = specification.Skip,
            Sort = specification.ToSortDefinition()
        };
    }

    public static FindOptions<T, TProjection> ToFindOptions<T, TProjection>(this ISpecification<T, TProjection> specification)
    {
        return new FindOptions<T, TProjection>
        {
            Limit = specification.Take,
            Skip = specification.Skip,
            Sort = specification.ToSortDefinition(),
            Projection = Builders<T>.Projection.Expression(specification.Selector),
        };
    }

    public static SortDefinition<T>? ToSortDefinition<T>(this ISpecification<T> specification)
    {
        if (!specification.OrderExpressions.Any())
            return null;

        using var enumerator = specification.OrderExpressions.GetEnumerator();
        enumerator.MoveNext();
        var expression = enumerator.Current;
        var sort = expression.OrderType == OrderTypeEnum.OrderBy || expression.OrderType == OrderTypeEnum.ThenBy
            ? Builders<T>.Sort.Ascending(expression.KeySelector)
            : Builders<T>.Sort.Descending(expression.KeySelector);
        while (enumerator.MoveNext())
        {
            expression = enumerator.Current;
            sort = expression.OrderType == OrderTypeEnum.OrderBy || expression.OrderType == OrderTypeEnum.ThenBy
                ? sort.Ascending(expression.KeySelector)
                : sort.Descending(expression.KeySelector);
        }

        return sort;
    }
}