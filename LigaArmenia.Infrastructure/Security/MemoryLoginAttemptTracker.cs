using LigaArmenia.Application.Interfaces.Security;
using Microsoft.Extensions.Caching.Memory;

namespace LigaArmenia.Infrastructure.Security;

public sealed class MemoryLoginAttemptTracker : ILoginAttemptTracker
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan WindowDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(10);

    private readonly IMemoryCache _cache;

    public MemoryLoginAttemptTracker(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool IsLockedOut(string key, out TimeSpan remaining)
    {
        remaining = TimeSpan.Zero;
        if (_cache.TryGetValue(GetCacheKey(key), out AttemptState? state) && state is not null)
        {
            if (state.LockedUntilUtc.HasValue && state.LockedUntilUtc.Value > DateTimeOffset.UtcNow)
            {
                remaining = state.LockedUntilUtc.Value - DateTimeOffset.UtcNow;
                return true;
            }
        }

        return false;
    }

    public void RecordFailure(string key)
    {
        var cacheKey = GetCacheKey(key);
        var state = _cache.Get<AttemptState>(cacheKey) ?? new AttemptState();

        state.FailedCount++;
        if (state.FailedCount >= MaxFailedAttempts)
        {
            state.LockedUntilUtc = DateTimeOffset.UtcNow.Add(LockoutDuration);
        }

        _cache.Set(cacheKey, state, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = WindowDuration
        });
    }

    public void Reset(string key)
    {
        _cache.Remove(GetCacheKey(key));
    }

    private static string GetCacheKey(string key) => $"sec:login-attempt:{key}";

    private sealed class AttemptState
    {
        public int FailedCount { get; set; }
        public DateTimeOffset? LockedUntilUtc { get; set; }
    }
}
