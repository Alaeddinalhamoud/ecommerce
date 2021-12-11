using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.FrontEnd.Models
{
    public class MVShopCategory
    {
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string goToAction { get; set; }
    }
}
