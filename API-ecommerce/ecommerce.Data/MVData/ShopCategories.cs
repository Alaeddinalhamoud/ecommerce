using System.Collections.Generic;

namespace ecommerce.Data.MVData
{
   public class ShopCategories
    {
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<Brand> brands { get; set; }
        public IEnumerable<ProductType> productTypes  { get; set; }
    }
}
