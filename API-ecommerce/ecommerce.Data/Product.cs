using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        public string createdBy { get; set; }        
        public string vendorId { get; set; }
        public bool isDeleted { get; set; }
        [Display(Name = "Old Price")]
        public double oldPrice { get; set; }
        [Display(Name = "Price")]
        public double price { get; set; }
        [Display(Name = "Category")]
        public int categoryId { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Tag")]
        public string tags { get; set; }
        public bool isApproved { get; set; }
        [Display(Name = "Youtube link")]
        public string videoUrl { get; set; }
        public double rating { get; set; }
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
        [Display(Name = "Product type")]
        public int productTypeId { get; set; }
        [DataType(DataType.Date)]
        public DateTime createDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public int qty { get; set; }
        [Display(Name = "Package Type")]
        public PackageType packageType { get; set; }
        public string shortDescription { get; set; }
        public bool  freeShipping { get; set; }
        public bool freeTax { get; set; }
        // public int frequency { get; set; } WE have moved it to new TB (Most Viewed Product)
    }
}
