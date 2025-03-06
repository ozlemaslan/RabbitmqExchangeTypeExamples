using RabbitMq.Common;
using RabbitMq.Producer;
using RabbitMq.Producer.ExchangeTypes.Direct;
using RabbitMq.Producer.ExchangeTypes.Fanout;
using RabbitMq.Producer.ExchangeTypes.Topic;
using RabbitMq.Producer.Interfaces;
using RabbitMQ.Client;

namespace Rabbitmq.Producer
{
    public static class ServiceRegisteration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory() { Uri = new Uri(configuration.GetConnectionString("RabbitMq")) });

            services.AddStackExchangeRedisCache(options => options.Configuration = "localhost:6379");
            
            services.AddSingleton<RabbitMqService>();

            services.AddScoped<IBaseExchangeType, DirectExchangeType>();
            services.AddScoped<IBaseExchangeType, FanoutExchangeType>();
            services.AddScoped<IBaseExchangeType, TopicExchangeType>();

            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}
