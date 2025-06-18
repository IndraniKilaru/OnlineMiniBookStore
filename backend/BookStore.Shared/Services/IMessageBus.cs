using System;
using System.Threading.Tasks;

namespace BookStore.Shared.Services
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
        Task PublishAsync<T>(string exchange, string routingKey, T message) where T : class;
        void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class;
        void Subscribe<T>(string exchange, string queueName, string routingKey, Func<T, Task> handler) where T : class;
    }
}
