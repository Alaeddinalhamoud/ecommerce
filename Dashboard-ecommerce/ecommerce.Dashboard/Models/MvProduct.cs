using ecommerce.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ecommerce.Dashboard.Models
{
    public class MvProduct
    {
        public int id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "The Name is required")]
        public string name { get; set; }
        [Display(Name = "Old Price")]
        public double oldPrice { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "The Price is required")]
        public double price { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "The Category is required")]
        public int categoryId { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "The Description is required")]
        public string description { get; set; }
        [Display(Name = "Tag")]
        public string tags { get; set; }      
        [Display(Name = "Youtube link")]
        public string videoUrl { get; set; }      
        [Display(Name = "Creation Country")]
        public Countries placeOfOrigin { get; set; }
        [Display(Name = "Model Number")]
        public string modelNumber { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime expiryDate { get; set; }
        [Display(Name = "Bar Code")]
        public string barcode { get; set; }
        [Display(Name = "Inventory Code")]
        public string inventoryCode { get; set; }
        [Display(Name = "Lot Number")]
        public string LotNumber { get; set; }
        [Display(Name = "Brand Name")]
        public int brandId { get; set; }
        [Required]
        [Display(Name = "Product type")]
        public int productTypeId { get; set; }     
        public bool updateImage { get; set; }
        [Display(Name = "Qty")]
        public int qty { get; set; }
        [Display(Name = "Package Type")]
        public PackageType packageType { get; set; }
        public string shortDescription { get; set; }
        [Display(Name = "Free Shipping?")]
        public bool freeShipping { get; set; }
        [Display(Name = "Free Tax?")]
        public bool freeTax { get; set; }

        //Used to fill the dropdownlist
        public SelectList productTypes { get; set; }
        public SelectList categories { get; set; }
        public SelectList brands { get; set; }
        public SelectList countries { get; set; }

    }

    public class MvProductDetails
    {
        public Product product { get; set; }
        public IQueryable<Media>  medias { get; set; }
        public IQueryable<ProductSpecification> productSpecifications { get; set; }
    }
}
