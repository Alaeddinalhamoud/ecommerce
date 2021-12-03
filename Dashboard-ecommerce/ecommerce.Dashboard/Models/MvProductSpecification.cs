using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Dashboard.Models
{
    public class MvProductSpecification
    {
        public int id { get; set; }
        [Required(ErrorMessage = "The Product is required")]
        public int productId { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "The Name is required")]
        public string name { get; set; }
        [Display(Name = "Value")]
        [Required(ErrorMessage = "The Value is required")]
        public string value { get; set; }      
    }
    //This class used to return list of ProductSpecification to be saved in once.
    public class MvProductSpecificationList
    {
        public List<MvProductSpecification> MvProductSpecifications { get; set; }
    }
}
