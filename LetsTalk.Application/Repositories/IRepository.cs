using Ardalis.Specification;

namespace LetsTalk.Repositories;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
    string CollectionName { get; }

    /// <summary>
    /// Finds an entity with the given primary key value.
    /// </summary>
    /// <param name="id">The value of the primary key for the entity to be found.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an entity with the given name.
    /// </summary>
    /// <param name="name">The name of the entity to be found.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
