using System;

namespace ecommerce.Data
{
   public class User
    {
        public string userId { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string roleId { get; set; }
        public string email { get; set; }
        public bool isVendor { get; set; }
        public bool isBlocked { get; set; }
        public string phoneNumber { get; set; }
        public DateTime dOB { get; set; }


    }
}
