using Microsoft.AspNetCore.Mvc;
using RabbitMq.Producer.ExchangeTypes.Direct;
using RabbitMq.Producer.ExchangeTypes.Fanout;
using RabbitMq.Producer.ExchangeTypes.Topic;
using RabbitMq.Producer.Interfaces;
using RabbitMQ.Client;

namespace RabbitMq.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController(IConnectionFactory connectionFactory) : ControllerBase
    {

        [HttpGet]
        public IActionResult SendMessage()
        {
            try
            {
                var exchange = FindExchangeType(ExchangeType.Topic);

                exchange.CreateExchangeAndQueueDeclare();

                exchange.SendToQueue();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok();
        }


        private IBaseExchangeType FindExchangeType(string exchangeType)
        {
            switch (exchangeType)
            {
                case ExchangeType.Direct:
                    return new DirectExchangeType(connectionFactory);
                case ExchangeType.Fanout:
                    return new FanoutExchangeType(connectionFactory);
                case ExchangeType.Topic:
                    return new TopicExchangeType(connectionFactory);
                default:
                    throw new Exception("exchange type has not found.");
            }
        }
    }
}
