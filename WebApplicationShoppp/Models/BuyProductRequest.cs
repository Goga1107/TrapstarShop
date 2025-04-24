namespace WebApplicationShoppp.Models
{
    public class BuyProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }  // This can be used to calculate the total price
    }

}
