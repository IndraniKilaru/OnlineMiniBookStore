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
    public class UserRegisteredConsumer : BackgroundService
    {
        private readonly ILogger<UserRegisteredConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;        public UserRegisteredConsumer(
            ILogger<UserRegisteredConsumer> logger,
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
            _channel.ExchangeDeclare("bookstore.users", ExchangeType.Topic, durable: true);
            _channel.QueueDeclare("user.notifications", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("user.notifications", "bookstore.users", "user.registered");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(message);

                    if (userEvent != null)
                    {
                        await ProcessUserRegisteredEvent(userEvent);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize UserRegisteredEvent");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing user registered event");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume("user.notifications", false, consumer);
            _logger.LogInformation("UserRegisteredConsumer started and listening for messages...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessUserRegisteredEvent(UserRegisteredEvent userEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            _logger.LogInformation($"Processing user registered event for {userEvent.Email}");
            
            await emailService.SendWelcomeEmailAsync(userEvent.Email, userEvent.Username);
            
            _logger.LogInformation($"Welcome email sent to {userEvent.Email}");
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
