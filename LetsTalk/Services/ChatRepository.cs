using LetsTalk.Interfaces;
using LetsTalk.Models;
using System.Collections.Concurrent;

namespace LetsTalk.Services;

public sealed class ChatRepository : IChatRepository
{
    private readonly ConcurrentDictionary<string, ChatRoom> _chatRooms;

    public ChatRepository()
    {
        var generalId = Guid.NewGuid().ToString();
        _chatRooms = new()
        {
            [generalId] = new ChatRoom { Id = generalId, Name = "General" }
        };
    }

    public async Task<IEnumerable<ChatRoom>> GetAllAsync() => await Task.FromResult(_chatRooms.Values);
    public async Task<ChatRoom?> GetAsync(string id) => await Task.FromResult(_chatRooms.GetValueOrDefault(id));
    public async Task InsertAsync(ChatRoom entity) => await Task.FromResult(_chatRooms.TryAdd(entity.Id, entity));
    public async Task UpdateAsync(ChatRoom entity)
    {
        if (_chatRooms.ContainsKey(entity.Id))
            _chatRooms[entity.Id] = entity;
        await Task.CompletedTask;
    }
    public async Task UpsertAsync(ChatRoom entity) => await Task.FromResult(_chatRooms[entity.Id] = entity);
    public async Task DeleteAsync(string id)
    {
        if (_chatRooms.TryRemove(id, out var chatRoom))
            chatRoom.CancelAllSubscritpions();
        await Task.CompletedTask;
    }
}
