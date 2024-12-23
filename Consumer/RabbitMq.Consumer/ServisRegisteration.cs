using RabbitMQ.Client;
using RabbitMq.Common;

namespace RabbitMq.Consumer
{
    public static class ServisRegisteration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory() { Uri = new Uri(configuration.GetConnectionString("RabbitMq")) });

            services.AddSingleton<RabbitMqService>();


            return services;
        }
    }
}
