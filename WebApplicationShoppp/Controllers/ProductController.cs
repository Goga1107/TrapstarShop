using Microsoft.AspNetCore.Mvc;
using WebApplicationShoppp.Models;
using WebApplicationShoppp.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplicationShoppp.Data;
using Microsoft.AspNetCore.Identity;

namespace WebApplicationShoppp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IproductRepository _productRepository;
        private readonly ProductDbContext _context;
        private readonly IEmailInterface _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(IproductRepository productRepository, ProductDbContext context,IEmailInterface emailInterface,UserManager<IdentityUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _emailSender = emailInterface;
            _userManager = userManager;
            //  _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetProducts()
        {
            var all = await _productRepository.GetAllProductsAsync();
            return Ok(all);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductModel productModel, IFormFile imageFile)
        {
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productRepository.AddProductAsync(productModel, imageFile);
            return Ok("Product added successfully");
        }


        [HttpGet("id")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var prod = await _productRepository.GetProductByIdAsync(id);
            if (prod == null)
            {
                return NotFound("Product not found");
            }
            return Ok(prod);
        }

        
        [HttpGet("search/manufacturer")]
        public async Task<IActionResult> SearchByManufacturer(string name)
        {
            var prod = await _productRepository.SearchByManufacturerAsync(name);
            if (prod == null || prod.Count == 0)
            {
                return NotFound("No products found for the specified manufacturer");
            }
            return Ok(prod);
        }

        [HttpGet("minPrice/maxPrice")]
        public async Task<IActionResult> SearchByPrice(int min, int max)
        {
            var prod = await _productRepository.SearchByPriceRangeAsync(min, max);
            if (prod == null || prod.Count == 0)
            {
                return NotFound("No products found within the given price range");
            }
            return Ok(prod);
        }

        [HttpGet("search/category")]
        public async Task<IActionResult> SearchByCategory(string category)
        {
            var prod = await _productRepository.SearchByCategoryAsync(category);
            if (prod == null || prod.Count == 0)
            {
                return NotFound("No products found for the specified category");
            }
            return Ok(prod);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch");
            }

            try
            {
                await _productRepository.EditProductAsync(id, product);
                return Ok("Product updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("sortByPriceAscending")]
        public async Task<IActionResult> SortByPriceAsc()
        {
            var prod = await _productRepository.SortByPriceAsc();
            return Ok(prod);
        }
        [HttpGet("sortByPriceDescending")]
        public async Task<IActionResult> SortByPriceDesc()
        {
            var prod = await _productRepository.SortByPriceDesc();
            return Ok(prod);
        }


        [HttpDelete("delete/id")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            await _productRepository.DeleteProductAsync(id);
            return Ok("Product deleted successfully");
        }

        [HttpPost("return/orderId")]
        public async Task<IActionResult> ReturnProduct(int orderId)
        {



            var order = await _context.Orders
                .Include(o => o.Product) 
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

         
            var timeSincePurchase = DateTime.UtcNow - order.PurchaseDate;

            
            if (timeSincePurchase.TotalMinutes > 1)
            {
                return BadRequest("Return time has expired. Returns are only allowed within 1 minute of purchase.");
            }
            var product = order.Product;
            product.Quantity += order.Quantity;
            _context.Products.Update(product);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
      

            return Ok("Product returned successfully");
        }
    }
}

