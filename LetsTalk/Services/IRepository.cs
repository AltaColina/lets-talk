namespace LetsTalk.Services;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? Get(string id);
    void Insert(T entity);
    void Update(T entity);
    void Upsert(T entity);
    void Delete(string id);
}
