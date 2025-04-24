using WebApplicationShoppp.Data;
using WebApplicationShoppp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApplicationShoppp.Services
{
    public class ProductRepository : IproductRepository
    {
        private readonly ProductDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductRepository(ProductDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GetUserEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }
        public async Task AddProductAsync(ProductModel productModel, IFormFile imageFile)
        {
            var product = new Product
            {
                Manufacturer = productModel.Manufacturer,
                Model = productModel.Model,
                Category = productModel.Category,
                Description = productModel.Description,
                Price = productModel.Price,
                Quantity = productModel.Quantity,
                Year = productModel.Year,
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                var extension = Path.GetExtension(imageFile.FileName);
                var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                var imagePath = Path.Combine("wwwroot", "images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                product.Image = $"/images/{uniqueFileName}"; // Store the path to the image
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }



        public async Task<ICollection<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return product;
        }

        public async Task<ICollection<Product>> SearchByCategoryAsync(string categoryName)
        {
            var products = await _context.Products.Where(p => p.Category == categoryName).ToListAsync();
            if (products == null || products.Count == 0)
            {
                throw new Exception("No products found for this category");
            }
            return products;
        }

        public async Task<ICollection<Product>> SearchByManufacturerAsync(string manuf)
        {
            var products = await _context.Products.Where(p => p.Manufacturer == manuf).ToListAsync();
            if (products == null || products.Count == 0)
            {
                throw new Exception("No products found for this manufacturer");
            }
            return products;
        }

        public async Task<ICollection<Product>> SearchByPriceRangeAsync(int minprice, int maxprice)
        {
            var products = await _context.Products.Where(p => p.Price >= minprice && p.Price <= maxprice).ToListAsync();
            if (products == null || products.Count == 0)
            {
                throw new Exception("No products found within the given price range.");
            }
            return products;
        }

       

        public async Task EditProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            // Update fields
         
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.Manufacturer = product.Manufacturer;
            existingProduct.Model = product.Model;
            existingProduct.Price = product.Price;

            _context.Entry(existingProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(int id)
        {
            var forRemove = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (forRemove == null)
            {
                throw new Exception("Product not found with this ID");
            }

            _context.Products.Remove(forRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Product>> SortByPriceAsc()
        {
            var fororder = await _context.Products.OrderBy(p => p.Price).ToListAsync();

            return fororder;
        }
        public async Task<ICollection<Product>> SortByPriceDesc()
        {
            var fororder = await _context.Products.OrderByDescending(p => p.Price).ToListAsync();

            return fororder;
        }

    }
}
