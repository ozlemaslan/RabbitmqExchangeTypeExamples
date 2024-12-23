namespace RabbitMq.Producer.Interfaces
{
    public interface IBaseExchangeType
    {
        void CreateExchangeAndQueueDeclare();
        void SendToQueue();
    }
}
