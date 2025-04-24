using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace WebApplicationShoppp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string UserId { get; set; }

        public string Size { get; set; }
        public string Color { get; set; }

        // Navigation 
        [JsonIgnore] 
        public IdentityUser User { get; set; }
       

        public Product Product { get; set; }
    }
}
