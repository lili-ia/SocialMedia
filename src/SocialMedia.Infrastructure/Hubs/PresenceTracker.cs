namespace Infrastructure.Hubs;

public class PresenceTracker
{
    private readonly Dictionary<Guid, HashSet<string>> _connections = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task UserConnectedAsync(Guid userId, string connectionId)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_connections.TryGetValue(userId, out var connections))
            {
                connections = [];
                _connections[userId] = connections;
            }

            connections.Add(connectionId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UserDisconnectedAsync(Guid userId, string connectionId)
    {
        await _lock.WaitAsync();
        try
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _connections.Remove(userId);
                }
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        await _lock.WaitAsync();
        try
        {
            return _connections.ContainsKey(userId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<Guid>> GetOnlineUsersAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return _connections.Keys.ToList();
        }
        finally
        {
            _lock.Release();
        }
    }
}