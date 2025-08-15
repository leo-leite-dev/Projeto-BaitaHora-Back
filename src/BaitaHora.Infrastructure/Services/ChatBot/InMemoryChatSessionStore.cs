using System.Collections.Concurrent;
using BaitaHora.Application.IServices.IChatbot;

namespace BaitaHora.Infrastructure.Services.Chat
{
    public sealed class InMemoryChatSessionStore : IChatSessionStore
    {
        private static readonly ConcurrentDictionary<string, ChatSession> _mem = new();

        public Task<ChatSession?> GetAsync(string key, CancellationToken ct = default)
            => Task.FromResult(_mem.TryGetValue(key, out var s) ? s : null);

        public Task SetAsync(ChatSession session, CancellationToken ct = default)
        {
            _mem[session.Key] = session; return Task.CompletedTask;
        }

        public Task ClearAsync(string key, CancellationToken ct = default)
        {
            _mem.TryRemove(key, out _); return Task.CompletedTask;
        }
    }
}