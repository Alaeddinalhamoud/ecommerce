using ecommerce.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MvApplicationUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public DateTime dOB { get; set; }
        public Genders gender { get; set; }       
        public bool isBlocked { get; set; }
        public bool isVendor { get; set; }
        public bool isAdmin { get; set; }
        public bool isCustomer { get; set; }
    }
}
