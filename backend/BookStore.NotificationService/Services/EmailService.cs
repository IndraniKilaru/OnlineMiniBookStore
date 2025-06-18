using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BookStore.NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string email, string userName)
        {
            var subject = "Welcome to Mini Online Bookstore!";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome {userName}!</h2>
                    <p>Thank you for joining Mini Online Bookstore. We're excited to have you as part of our community!</p>
                    <p>Start exploring our vast collection of books and find your next great read.</p>
                    <br>
                    <p>Happy Reading!</p>
                    <p>The Mini Bookstore Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendOrderConfirmationEmailAsync(string email, string userName, int orderId, decimal totalAmount)
        {
            var subject = $"Order Confirmation #{orderId} - Mini Online Bookstore";
            var body = $@"
                <html>
                <body>
                    <h2>Order Confirmation</h2>
                    <p>Hi {userName},</p>
                    <p>Thank you for your order! We've received your order and it's being processed.</p>
                    <br>
                    <h3>Order Details:</h3>
                    <p><strong>Order Number:</strong> #{orderId}</p>
                    <p><strong>Total Amount:</strong> ${totalAmount:F2}</p>
                    <br>
                    <p>You'll receive another email when your order ships.</p>
                    <br>
                    <p>Thank you for shopping with us!</p>
                    <p>The Mini Bookstore Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"] ?? "Mini Bookstore",
                    _configuration["Email:FromAddress"] ?? "noreply@minibookstore.com"
                ));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                // For development, we'll just log the email instead of actually sending it
                // In production, you would configure SMTP settings
                var isDevelopment = _configuration.GetValue<bool>("Email:UseMockSender", true);
                
                if (isDevelopment)
                {
                    _logger.LogInformation($"[MOCK EMAIL] To: {to}");
                    _logger.LogInformation($"[MOCK EMAIL] Subject: {subject}");
                    _logger.LogInformation($"[MOCK EMAIL] Body: {body}");
                    await Task.Delay(100); // Simulate email sending delay
                }
                else
                {
                    using var client = new SmtpClient();
                    await client.ConnectAsync(
                        _configuration["Email:SmtpHost"], 
                        int.Parse(_configuration["Email:SmtpPort"] ?? "587"), 
                        false
                    );
                    
                    await client.AuthenticateAsync(
                        _configuration["Email:Username"], 
                        _configuration["Email:Password"]
                    );
                    
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
            }
        }
    }
}
