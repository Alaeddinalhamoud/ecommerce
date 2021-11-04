using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class MostViewedProduct
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        public int frequency { get; set; }
        public DateTime lastVisitDate { get; set; }
    }
}
