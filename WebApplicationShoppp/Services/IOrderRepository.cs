using WebApplicationShoppp.Models;

namespace WebApplicationShoppp.Services
{
    public interface IOrderRepository
    {
         Task<Order> BuyProductAsync(int productId, int quantity, string size, string color,string userId);
        Task<ICollection<Order>> GetOrdersAsync();

        Task<List<Order>> GetOrdersByUserAsync(string userId);

     
    }
}
