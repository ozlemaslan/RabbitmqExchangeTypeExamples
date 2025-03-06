namespace RabbitMq.Common
{
    public interface ICacheService
    {
        public Task<T> GetAsync<T>(string key);

        public Task SetAsync<T>(string key, T item, Action<CacheSettings> config);

        public Task RemoveAsync(string key);
    }

    public class CacheSettings
    {
        public int AbsoluteTime { get; set; }
        public int SlidingTime { get; set; }
    }
}
