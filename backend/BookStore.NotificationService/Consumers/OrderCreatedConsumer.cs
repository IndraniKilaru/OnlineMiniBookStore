using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using BookStore.Shared.Events;
using BookStore.NotificationService.Services;

namespace BookStore.NotificationService.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;        public OrderCreatedConsumer(
            ILogger<OrderCreatedConsumer> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetConnectionString("RabbitMQ:Host") ?? "localhost",
                Port = int.Parse(configuration.GetConnectionString("RabbitMQ:Port") ?? "5672"),
                UserName = configuration.GetConnectionString("RabbitMQ:Username") ?? "bookstore",
                Password = configuration.GetConnectionString("RabbitMQ:Password") ?? "bookstore123"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Declare the exchange and queue
            _channel.ExchangeDeclare("bookstore.orders", ExchangeType.Topic, durable: true);
            _channel.QueueDeclare("order.notifications", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("order.notifications", "bookstore.orders", "order.created");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                    if (orderEvent != null)
                    {
                        await ProcessOrderCreatedEvent(orderEvent);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize OrderCreatedEvent");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing order created event");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume("order.notifications", false, consumer);
            _logger.LogInformation("OrderCreatedConsumer started and listening for messages...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessOrderCreatedEvent(OrderCreatedEvent orderEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            _logger.LogInformation($"Processing order created event for Order #{orderEvent.OrderId}");
            
            // Extract customer name from email (simple approach)
            var customerName = orderEvent.CustomerEmail.Split('@')[0];
            
            await emailService.SendOrderConfirmationEmailAsync(
                orderEvent.CustomerEmail, 
                customerName, 
                orderEvent.OrderId, 
                orderEvent.TotalAmount
            );
            
            _logger.LogInformation($"Order confirmation email sent to {orderEvent.CustomerEmail}");
            
            // Additional processing could include:
            // - Updating inventory
            // - Sending notifications to warehouse
            // - Analytics tracking
            // - Integration with payment processors
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
