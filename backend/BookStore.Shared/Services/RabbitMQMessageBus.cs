using BookStore.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore.Shared.Services
{
    public class RabbitMQMessageBus : IMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQMessageBus> _logger;        public RabbitMQMessageBus(IConfiguration configuration, ILogger<RabbitMQMessageBus> logger)
        {
            _logger = logger;
            
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:Username"] ?? "bookstore",
                Password = configuration["RabbitMQ:Password"] ?? "bookstore123"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public async Task PublishAsync<T>(string queueName, T message) where T : class
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: messageBody);
            
            _logger.LogInformation($"Published message to queue {queueName}: {typeof(T).Name}");
            await Task.CompletedTask;
        }

        public async Task PublishAsync<T>(string exchange, string routingKey, T message) where T : class
        {
            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: properties, body: messageBody);
            
            _logger.LogInformation($"Published message to exchange {exchange} with routing key {routingKey}: {typeof(T).Name}");
            await Task.CompletedTask;
        }

        public void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        await handler(message);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message from queue {queueName}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"Subscribed to queue {queueName}");
        }

        public void Subscribe<T>(string exchange, string queueName, string routingKey, Func<T, Task> handler) where T : class
        {
            _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        await handler(message);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message from exchange {exchange}, queue {queueName}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"Subscribed to exchange {exchange}, queue {queueName}, routing key {routingKey}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
