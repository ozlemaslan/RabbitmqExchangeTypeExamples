using Microsoft.AspNetCore.Mvc;
using RabbitMq.Common;
using RabbitMq.Producer.ExchangeTypes.Direct;
using RabbitMq.Producer.ExchangeTypes.Fanout;
using RabbitMq.Producer.ExchangeTypes.Topic;
using RabbitMq.Producer.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMq.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController(IConnectionFactory connectionFactory, ICacheService cacheService) : ControllerBase
    {

        [HttpGet("SendMessageQueue")]
        public async Task<IActionResult> SendMessage()
        {
            try
            {
                var exchange = FindExchangeType(ExchangeType.Direct);

                exchange.CreateExchangeAndQueueDeclare();

                exchange.SendToQueue();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //var list = await ReadMessagesFromCache();

            //Console.WriteLine("Cacheden okunan veriler: ", list);

            return Ok();
        }

        [HttpGet("GetReadCacheValues")]
        public async Task<IActionResult> GetReadCacheValues()
        {
            var list = await ReadMessagesFromCache();

            Console.WriteLine("Cacheden okunan veriler: \n", list);

            return Ok(list);
        }

        private IBaseExchangeType FindExchangeType(string exchangeType)
        {
            switch (exchangeType)
            {
                case ExchangeType.Direct:
                    return new DirectExchangeType(connectionFactory, cacheService);
                case ExchangeType.Fanout:
                    return new FanoutExchangeType(connectionFactory);
                case ExchangeType.Topic:
                    return new TopicExchangeType(connectionFactory);
                default:
                    throw new Exception("exchange type has not found.");
            }
        }

        private async Task<List<string>> ReadMessagesFromCache()
        {
            var cachedMessages = new List<string>();

            string[] queues = { "direct-queue-1", "direct-queue-2", "direct-queue-3" };
            foreach (var queueKey in queues)
            {
                string cacheKey = $"cache-{queueKey}";
                byte[] cachedData = await cacheService.GetAsync<byte[]>(cacheKey);

                if (cachedData != null)
                {
                    string message = Encoding.UTF8.GetString(cachedData);
                    cachedMessages.Add(message);
                }
            }

            return cachedMessages;
        }
    }
}
