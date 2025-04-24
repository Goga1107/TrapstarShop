using System.ComponentModel.DataAnnotations;

namespace WebApplicationShoppp.Models
{
    public class ProductModel
    {

        [Required]
        public string Manufacturer { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public DateTime Year { get; set; }
        [Required]
        public int Quantity { get; set; }

       
    }
}
