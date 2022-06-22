﻿using LiteDB;

namespace LetsTalk.Services;

public abstract class LiteDatabaseRepository<T> : IRepository<T>
{
    protected ILiteCollection<T> Collection { get; }

    public LiteDatabaseRepository(LiteDatabase liteDatabase) => Collection = liteDatabase.GetCollection<T>();

    public IEnumerable<T> GetAll() => Collection.FindAll();
    public T? Get(string id) => Collection.FindById(id);

    public void Insert(T entity) => Collection.Insert(entity);
    public void Update(T entity) => Collection.Update(entity);
    public void Upsert(T entity) => Collection.Upsert(entity);
    public void Delete(string id) => Collection.Delete(id);
}
