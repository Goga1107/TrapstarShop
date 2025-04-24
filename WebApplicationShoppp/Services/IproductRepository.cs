using Microsoft.EntityFrameworkCore;
using WebApplicationShoppp.Models;

namespace WebApplicationShoppp.Services
{
    public interface IproductRepository
    {

        Task AddProductAsync(ProductModel productModel,IFormFile imageFile);
        Task<string> GetUserEmailByIdAsync(string userId);

        Task<ICollection<Product>> GetAllProductsAsync();

        Task<Product> GetProductByIdAsync(int id);

        Task<ICollection<Product>> SearchByCategoryAsync(string categoryName);

        Task<ICollection<Product>> SearchByManufacturerAsync(string manuf);

        Task<ICollection<Product>> SortByPriceAsc();
        Task<ICollection<Product>> SortByPriceDesc();



        Task<ICollection<Product>> SearchByPriceRangeAsync(int minprice, int maxprice);

        Task EditProductAsync(int id,Product product);

        Task DeleteProductAsync(int id);



    }
}
