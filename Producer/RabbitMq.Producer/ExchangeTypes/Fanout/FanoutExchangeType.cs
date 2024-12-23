using RabbitMq.Common;
using RabbitMq.Producer.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMq.Producer.ExchangeTypes.Fanout
{
    public class FanoutExchangeType : IBaseExchangeType
    {
        private readonly RabbitMqService _rabbitMqService;

        private const string exchangeName = "fanout-exchange";

        private const string queueName1 = "fanout-queue-1";
        private const string queueName2 = "fanout-queue-2";
        private const string queueName3 = "fanout-queue-3";

        private const string routingKey = ""; //fanout ta routing key yok tüm queuelara mesaj gider.

        private const string message1 = "1.fanout has sent to Rabbitmq.";
        private const string message2 = "2.fanout has sent to Rabbitmq.";
        private const string message3 = "3.fanout has sent to Rabbitmq.";

        private IModel _channel;
        public FanoutExchangeType(IConnectionFactory connectionFactory)
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
        }
        public void CreateExchangeAndQueueDeclare()
        {
            if (_channel != null)
            {
                // direct exchange oluşturduk.
                _channel.ExchangeDeclare(exchange: exchangeName, ExchangeType.Fanout, true, false, null);

                _channel.QueueDeclare(queueName1, true, false, false, null);
                _channel.QueueDeclare(queueName2, true, false, false, null);
                _channel.QueueDeclare(queueName3, true, false, false, null);

                _channel.QueueBind(queueName1, exchangeName, routingKey, null);
                _channel.QueueBind(queueName2, exchangeName, routingKey, null);
                _channel.QueueBind(queueName3, exchangeName, routingKey, null);
            }
            else throw new Exception("It has not connected to rabbitmq.");

        }

        public void SendToQueue()
        {
            if (_channel != null)
            {
                _channel.BasicPublish(exchangeName, routingKey, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message1));
                Console.Write($" Message Sent {message1}");

                _channel.BasicPublish(exchangeName, routingKey, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message2));
                Console.Write($" Message Sent {message1}");

                _channel.BasicPublish(exchangeName, routingKey, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message3));
                Console.Write($" Message Sent {message1}");
            }
            else throw new Exception("It has not connected to rabbitmq.");
        }
    }
}
