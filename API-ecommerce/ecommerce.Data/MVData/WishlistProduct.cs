namespace ecommerce.Data.MVData
{
   public class WishlistProduct
    {
        public int id { get; set; }       
        public int productId { get; set; }       
        public bool isDeleted { get; set; }        
        public string userId { get; set; }
        public string productName { get; set; }
        public int qty { get; set; }
        public string productImage { get; set; }
        public double price { get; set; }
    }
}
