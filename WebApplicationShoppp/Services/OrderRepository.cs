using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using WebApplicationShoppp.Data;
using WebApplicationShoppp.Models;

namespace WebApplicationShoppp.Services
{
    public class OrderRepository : IOrderRepository 
    {
        private readonly ProductDbContext _context;
        private readonly IEmailInterface _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderRepository(ProductDbContext context, IEmailInterface emailSender,UserManager<IdentityUser> userManager)
        {

            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }


        public async Task<ICollection<Order>> GetOrdersAsync ()
        {
            var order = await _context.Orders.Include(o => o.Product).ToListAsync();


            return  order;
        }

        public async Task<List<Order>> GetOrdersByUserAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Product)
                .ToListAsync();
        }
        public async Task<string> GetUserEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }
        public async Task<Order> BuyProductAsync(int productId, int quantity, string size, string color, string userId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            var totalPrice = product.Price * quantity;
            product.Quantity -= quantity;

            var order = new Order
            {
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PurchaseDate = DateTime.Now,
                Size = size,      
                Color = color,
                UserId = userId
            };

            _context.Update(product);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            var userEmail = await GetUserEmailByIdAsync(userId);

            await _emailSender.SendOrderConfirmationEmailAsync(userEmail, order);
            return order;
        }

    }
}
