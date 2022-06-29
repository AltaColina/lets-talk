using Grpc.Core;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow;

namespace LetsTalk.Models;

public sealed class ChatRoom
{
    private readonly BufferBlock<Message> _messageBuffer = new(new DataflowBlockOptions { EnsureOrdered = true });
    private readonly ConcurrentDictionary<string, IServerStreamWriter<Message>> _subscriptions = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    [NotNull] public string? Id { get; init; }
    [NotNull] public string? Name { get; init; }
    public CancellationToken CancellationToken { get => _cancellationTokenSource.Token; }

    public void CancelAllSubscritpions()
    {
        _cancellationTokenSource.Cancel();
        _subscriptions.Clear();
    }

    public async Task JoinAsync(string username, IServerStreamWriter<Message> responseStream)
    {
        if (_subscriptions.TryAdd(username, responseStream))
        {
            await QueueMessageAsync(new Message
            {
                ChatId = Id,
                Username = Name,
                Text = $"{username} has joined channel '{Name}'."
            });

            while (!CancellationToken.IsCancellationRequested && _subscriptions.ContainsKey(username))
            {
                // Because only one client will get the message from the buffer.
                var message = await _messageBuffer.ReceiveAsync();
                // This needs to send the message to all subscribed clients.
                await Task.WhenAll(_subscriptions.Values.Select(s => s.WriteAsync(message)));
            }
        }
    }

    public async Task LeaveAsync(string username)
    {
        if (_subscriptions.TryRemove(username, out _))
        {
            await QueueMessageAsync(new Message
            {
                ChatId = Id,
                Username = Name,
                Text = $"{username} has left channel '{Name}'."
            });
        }
    }

    public Task QueueMessageAsync(Message message)
    {
        if (message.ChatId == Id)
            _messageBuffer.Post(message);
        return Task.CompletedTask;
    }
}
