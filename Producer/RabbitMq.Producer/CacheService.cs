using Microsoft.Extensions.Caching.Distributed;
using RabbitMq.Common;
using System.Text;
using System.Text.Json;

namespace RabbitMq.Producer
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        /// <summary>
        /// byte[] dönecek şekilde metot yazıldı.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            byte[] dizi = await _distributedCache.GetAsync(key);

            if (dizi != null)
            {
                return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(dizi));
            }
            return default(T);

        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task SetAsync<T>(string key, T item, Action<CacheSettings> config)
        {
            var jsonData = JsonSerializer.Serialize(item);
            var dataAsBytes = Encoding.UTF8.GetBytes(jsonData);

            var cacheSettings = new CacheSettings();

            config(cacheSettings);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(cacheSettings.AbsoluteTime),
                SlidingExpiration = TimeSpan.FromMinutes(cacheSettings.SlidingTime)
            };

            await _distributedCache.SetAsync(key, dataAsBytes,options);
        }
    }
}
