using RabbitMq.Common;
using RabbitMq.Producer.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMq.Producer.ExchangeTypes.Topic
{
    public class TopicExchangeType : IBaseExchangeType
    {
        private readonly RabbitMqService _rabbitMqService;

        private const string exchangeName = "topic-exchange";

        private const string queueName1 = "topic-queue-1";
        private const string queueName2 = "topic-queue-2";
        private const string queueName3 = "topic-queue-3";

        private const string routingPattern1 = "ozlem.*";
        private const string routingPattern2 = "ozlem.aslan.#";
        private const string routingPattern3 = "aslan.ozlem.*";

        private const string routingKey1 = "ozlem.aslan";
        private const string routingKey2 = "ozlem.aslan.query";
        private const string routingKey3 = "aslan.ozlem.123";


        private const string message1 = "1. topic has sent to Rabbitmq.";
        private const string message2 = "2. topic has sent to Rabbitmq.";
        private const string message3 = "3. topic has sent to Rabbitmq.";


        private IModel _channel;
        public TopicExchangeType(IConnectionFactory connectionFactory)
        {

            _rabbitMqService = new RabbitMqService(connectionFactory);

            if (!_rabbitMqService.IsConnected)
            {
                _rabbitMqService.TryConnect();
            }

            _channel = _rabbitMqService.CreateModel();
        }
        public void CreateExchangeAndQueueDeclare()
        {
            if (_channel != null)
            {
                // direct exchange oluşturduk.
                _channel.ExchangeDeclare(exchange: exchangeName, ExchangeType.Topic, true, false, null);

                _channel.QueueDeclare(queueName1, true, false, false, null);
                _channel.QueueDeclare(queueName2, true, false, false, null);
                _channel.QueueDeclare(queueName3, true, false, false, null);

                _channel.QueueBind(queueName1, exchangeName, routingPattern1, null);
                _channel.QueueBind(queueName2, exchangeName, routingPattern2, null);
                _channel.QueueBind(queueName3, exchangeName, routingPattern3, null);
            }
            else throw new Exception("It has not connected to rabbitmq.");

        }

        public void SendToQueue()
        {
            if (_channel != null)
            {
                _channel.BasicPublish(exchangeName, routingKey1, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message1));
                Console.Write($" Message Sent {message1}");

                _channel.BasicPublish(exchangeName, routingKey2, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message2));
                Console.Write($" Message Sent {message1}");

                _channel.BasicPublish(exchangeName, routingKey3, _channel.CreateBasicProperties(), Encoding.UTF8.GetBytes(message3));
                Console.Write($" Message Sent {message1}");
            }
            else throw new Exception("It has not connected to rabbitmq.");
        }
    }
}
