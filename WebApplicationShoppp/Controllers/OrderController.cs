using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationShoppp.Services;

namespace WebApplicationShoppp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderController(IOrderRepository repository,UserManager<IdentityUser> userManager)
        {
            _orderRepository = repository;
            _userManager = userManager;
        }

        [HttpGet("myorders")]
        [Authorize]
         public async Task<IActionResult> GetMyOrders()
         {
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(userId);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            var myorders = await _orderRepository.GetOrdersByUserAsync(user.Id);
            return Ok(myorders);
         }

        [HttpGet("get")]
        public async Task<IActionResult> GetOrders()
        {
          
           var orders = await _orderRepository.GetOrdersAsync();
            return Ok(orders);

        }

        [HttpPost("buy")]
        [Authorize]
        public async Task<IActionResult> BuyProduct(int productId, int quantity,string size,string color)
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(emailClaim))
            {
                return Unauthorized("Email claim not found.");
            }

            var user = await _userManager.FindByEmailAsync(emailClaim);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            try
            {
                var order = await _orderRepository.BuyProductAsync(productId, quantity,size,color,user.Id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
