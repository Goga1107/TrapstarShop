using WebApplicationShoppp.Models;

namespace WebApplicationShoppp.Services
{
    public interface IEmailInterface
    {
        Task<string> SendOTPEmailAsync(string email, string otp);
        Task<string> SendOrderConfirmationEmailAsync(string email, Order order);
        Task<string> SendOrderRefundEmailAsync(string email, Order order);
    }
}
