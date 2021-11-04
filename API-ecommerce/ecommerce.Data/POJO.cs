using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
   public class POJO
    {
        
        public string id { get; set; }
        [Display(Name = "Number of effected records")]
        public int numberOfRows { get; set; }
        [Display(Name = "Flag")]
        public bool flag { get; set; }
        [Display(Name = "Message")]
        public string message { get; set; }
    } 
}
