using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace RabbitMq.Common
{
    public class RabbitMqService(IConnectionFactory _connectionFactory, int retryCount = 5) : IDisposable
    {
        private IConnection _connection;
        private object lock_object = new();
        private int RetryCount = retryCount;
        private bool _disposed;

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
        public void Dispose()
        {
            _disposed = true;
            _connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object) // lock
            {
                var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>()
                    .WaitAndRetry(this.RetryCount, r => TimeSpan.FromSeconds(Math.Pow(2, r)), (ex, ts) =>
                    {

                    });

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected && !_disposed) return true;
                return false;
            }
        }
    }
}
