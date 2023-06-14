using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Dcis.Amfi.Isf.Mock.Helpers
{
    public class MemCache : IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        public MemCache(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public User? GetUser(string internalId, string? host = null) =>
            GetOrCreate(
                internalId,
                cacheUser => GetUsers(host)?.FirstOrDefault(u => u.InternalIdentityId.Equals(internalId, StringComparison.InvariantCultureIgnoreCase) == true));

        public Entity? GetEntity(string identitier) =>
            GetOrCreate(
                identitier,
                cacheEntity => GetEntities()?.FirstOrDefault(e => e.Identifier?.Equals(identitier, StringComparison.InvariantCultureIgnoreCase) == true));

        public IList<User>? GetUsers(string? host = null) =>
            GetOrCreate(
                CacheKeys.UsersCacheKey,
                cacheUsers =>
                {
                    // set expiration
                    cacheUsers.SlidingExpiration = TimeSpan.FromHours(4);
                    cacheUsers.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8);

                    return JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(@".\App_Data\users.json"))?.Select( u => {

                        if (!string.IsNullOrWhiteSpace(host)) {
                            u.RedirectUrl = u.RedirectUrl?.Replace("{host}", host);
                            u.ClaimAuthUrl = u.ClaimAuthUrl?.Replace("{host}", host);
                        }

                        return u;
                    }).ToList();
                });

        public IList<Entity>? GetEntities() =>
            GetOrCreate(
                CacheKeys.EntitiesCacheKey,
                cacheEntities =>
                {
                    // set expiration
                    cacheEntities.SlidingExpiration = TimeSpan.FromHours(4);
                    cacheEntities.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8);

                    return JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText(@".\App_Data\entities.json"));
                });

        public void Remove(object key) => _memoryCache.Remove(key);

        public ICacheEntry CreateEntry(object key) => _memoryCache.CreateEntry(key);

        public bool TryGetValue(object key, out object value) => _memoryCache.TryGetValue(key, out value);

        public object Get(object key) => _memoryCache.Get(key); 
        
        public TItem Get<TItem>(object key) => _memoryCache.Get<TItem>(key);

        public TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory) => _memoryCache.GetOrCreate(key, factory);

        public Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory) => _memoryCache.GetOrCreateAsync(key, factory);

        public TItem Set<TItem>(object key, TItem value) => _memoryCache.Set(key, value);

        public TItem Set<TItem>(object key, TItem value, MemoryCacheEntryOptions options) => _memoryCache.Set(key, value, options);

        public TItem Set<TItem>(object key, TItem value, IChangeToken expirationToken) => _memoryCache.Set(key, value, expirationToken);

        public TItem Set<TItem>(object key, TItem value, DateTimeOffset absoluteExpiration) => _memoryCache.Set(key, value, absoluteExpiration);

        public TItem Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow) => _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);

        public bool TryGetValue<TItem>(object key, out TItem value) => _memoryCache.TryGetValue(key, out value);

        public void Dispose() => _memoryCache.Dispose();
    }
}
