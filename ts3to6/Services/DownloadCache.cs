using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace ts3to6.Services;

/// <summary>
/// Singleton in-memory store for generated TS6 database files.
/// Retention duration is controlled by the <c>RETENTION_MINUTES</c> environment variable
/// (default: 60 minutes).
/// </summary>
public sealed class DownloadCache
{
    private readonly TimeSpan _ttl;
    private readonly ConcurrentDictionary<string, Entry> _store = new();

    private sealed record Entry(byte[] Data, DateTime Expires);

    public DownloadCache(IConfiguration config)
    {
        int minutes = config.GetValue<int>("RETENTION_MINUTES", 60);
        _ttl = TimeSpan.FromMinutes(Math.Max(1, minutes));
    }

    /// <summary>Persists <paramref name="data"/> and returns the opaque download ID.</summary>
    public string Store(byte[] data)
    {
        string id = Guid.NewGuid().ToString("N");
        _store[id] = new Entry(data, DateTime.UtcNow + _ttl);
        Cleanup();
        return id;
    }

    /// <summary>Returns the raw bytes for <paramref name="id"/>, or <c>null</c> if expired or not found.</summary>
    public byte[]? Get(string id)
    {
        if (!_store.TryGetValue(id, out var entry)) return null;
        if (entry.Expires <= DateTime.UtcNow) { _store.TryRemove(id, out _); return null; }
        return entry.Data;
    }

    /// <summary>Returns the configured TTL, for display purposes.</summary>
    public TimeSpan Ttl => _ttl;

    private void Cleanup()
    {
        DateTime now = DateTime.UtcNow;
        foreach (var (key, entry) in _store)
            if (entry.Expires <= now)
                _store.TryRemove(key, out _);
    }
}
