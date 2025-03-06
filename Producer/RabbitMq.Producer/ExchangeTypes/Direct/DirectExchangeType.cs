using RabbitMq.Common;
using RabbitMq.Producer.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMq.Producer.ExchangeTypes.Direct
{
    public class DirectExchangeType : IBaseExchangeType
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly ICacheService _cacheService;

        private const string exchangeName = "direct-exchange";

        private const string queueName1 = "direct-queue-1";
        private const string queueName2 = "direct-queue-2";
        private const string queueName3 = "direct-queue-3";

        private const string routingKey1 = "direct-routingkey-1";
        private const string routingKey2 = "direct-routingkey-2";
        private const string routingKey3 = "direct-routingkey-3";

        private const string message1 = "1. direct has sent to Rabbitmq.";
        private const string message2 = "2. direct has sent to Rabbitmq.";
        private const string message3 = "3. direct has sent to Rabbitmq.";

        private const string cacheKey = $"cache-";

        private IModel _channel;
        public DirectExchangeType(IConnectionFactory connectionFactory, ICacheService cacheService)
        {

            _rabbitMqService = new RabbitMqService(connectionFactory);

            if (!_rabbitMqService.IsConnected)//ilk başta hiç rabbitte bağlantı yoksa git polly ile 5 kere tekrar dene.
            {
                bool isConnected = _rabbitMqService.TryConnect(); // buradan true gelirse bağlantı var demek
                if (isConnected)
                {
                    _channel = _rabbitMqService.CreateModel();
                }
            }
            else _channel = _rabbitMqService.CreateModel();

            _cacheService = cacheService;

        }
        public void CreateExchangeAndQueueDeclare()
        {
            if (_channel!=null)
            {
                // direct exchange oluşturduk.
                _channel.ExchangeDeclare(exchange: exchangeName, ExchangeType.Direct, true, false, null);

                _channel.QueueDeclare(queueName1, true, false, false, null);
                _channel.QueueDeclare(queueName2, true, false, false, null);
                _channel.QueueDeclare(queueName3, true, false, false, null);

                _channel.QueueBind(queueName1, exchangeName, routingKey1, null);
                _channel.QueueBind(queueName2, exchangeName, routingKey2, null);
                _channel.QueueBind(queueName3, exchangeName, routingKey3, null);
            }
            else throw new Exception("It has not connected to rabbitmq.");

        }

        public async void SendToQueue()
        {
            if (_channel!=null)
            {
                _channel.BasicPublish(exchangeName, routingKey1, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message1));

                await _cacheService.SetAsync(cacheKey+queueName1, Encoding.UTF8.GetBytes(message1), settings =>
                {
                    settings.AbsoluteTime = 10; 
                    settings.SlidingTime = 5;  
                });

                Console.Write($" Message Sent {message1}");

                _channel.BasicPublish(exchangeName, routingKey2, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message2));

                await _cacheService.SetAsync(cacheKey+queueName2, Encoding.UTF8.GetBytes(message2), settings =>
                {
                    settings.AbsoluteTime = 10;
                    settings.SlidingTime = 5;
                });

                Console.Write($" Message Sent {message2}");

                _channel.BasicPublish(exchangeName, routingKey3, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message3));


                await _cacheService.SetAsync(cacheKey + queueName3, Encoding.UTF8.GetBytes(message3), settings =>
                {
                    settings.AbsoluteTime = 10;
                    settings.SlidingTime = 5;
                });
                Console.Write($" Message Sent {message3}");
            }
            else throw new Exception("It has not connected to rabbitmq.");
        }
    }
}
