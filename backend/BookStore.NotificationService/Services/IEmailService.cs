using System.Threading.Tasks;

namespace BookStore.NotificationService.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string email, string userName);
        Task SendOrderConfirmationEmailAsync(string email, string userName, int orderId, decimal totalAmount);
        Task SendEmailAsync(string to, string subject, string body);
    }
}
