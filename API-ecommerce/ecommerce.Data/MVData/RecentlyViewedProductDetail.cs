namespace ecommerce.Data.MVData
{
    public class RecentlyViewedProductDetail
    {
        public int id { get; set; }
        public int productId { get; set; } 
        public string userId { get; set; }
        public string productName { get; set; }
        public double rating { get; set; }
        public string productImage { get; set; }
        public double oldPrice { get; set; }
        public double price { get; set; }
        public int qty { get; set; }
    }
}
