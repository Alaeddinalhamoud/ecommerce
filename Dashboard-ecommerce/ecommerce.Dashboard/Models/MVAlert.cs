using ecommerce.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MVAlert
    {
        public int id { get; set; }
        [Required, DisplayName("Title")]
        public string title { get; set; }
        [Required, DisplayName("Body")]
        public string body { get; set; }
        [Required, DisplayName("Alert Type")]
        public AlertType alertType { get; set; }
        [DisplayName("Enabled")]
        public bool isEnabled { get; set; }
    }
}
