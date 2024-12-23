using Microsoft.AspNetCore.Mvc;
using RabbitMq.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMq.Consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumeController(RabbitMqService _rabbitMqService) : ControllerBase
    {
        private IModel _channel;

        [HttpGet]
        public IActionResult ConsumeTheMessage()
        {
            try
            {
                List<string> queueNames = FindQueueName(ExchangeType.Fanout);

                string queueName = "";

                if (!_rabbitMqService.IsConnected)
                {
                    bool isConnected = _rabbitMqService.TryConnect();
                    if (isConnected)
                    {
                        _channel = _rabbitMqService.CreateModel();
                    }
                }
                else _channel = _rabbitMqService.CreateModel();

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Consumer_Received;

                foreach (var item in queueNames)
                {
                    queueName = item;

                    _channel.BasicConsume(queueName, true, consumer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok();
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _channel.BasicAck(e.DeliveryTag, multiple: false);
        }

        private List<string> FindQueueName(string exchangeType)
        {
            List<string> queueNames = new List<string>();

            switch (exchangeType)
            {
                case ExchangeType.Direct:
                    for (int i = 1; i < 4; i++)
                    {
                        queueNames.Add("direct-queue-" + i);
                    }
                    return queueNames;
                case ExchangeType.Fanout:
                    for (int i = 1; i < 4; i++)
                    {
                        queueNames.Add("fanout-queue-" + i);
                    }
                    return queueNames;
                case ExchangeType.Topic:
                    for (int i = 1; i < 4; i++)
                    {
                        queueNames.Add("topic-queue-" + i);
                    }
                    return queueNames;
                default:
                    throw new Exception("queueNames has not found.");
            }
        }
    }
}
