using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
  public class RecentlyViewedProduct
    {
        [Key]
        public int id { get; set; }
        public string userId { get; set; }
        public int productId { get; set;}
        public DateTime createDate { get; set; }     
        public DateTime updateDate { get; set; }
    }
}
