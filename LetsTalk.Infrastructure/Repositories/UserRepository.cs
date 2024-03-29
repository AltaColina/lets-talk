﻿using LetsTalk.Users;
using MongoDB.Driver;

namespace LetsTalk.Repositories;

internal sealed class UserRepository : MongoEntityRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database) : base(database)
    {
    }
}