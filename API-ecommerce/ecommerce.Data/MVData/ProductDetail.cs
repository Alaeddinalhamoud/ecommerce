using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data.MVData
{
    public class ProductDetail
    {
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
        [Display(Name = "Creation Date")]
        public DateTime createDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime updateDate { get; set; }
        public string modifiedBy { get; set; }
        public int qty { get; set; }
        public PackageType packageType { get; set; }
        public string shortDescription { get; set; }
        public string brandName { get; set; }        
        public string categoryName { get; set; }
        public IEnumerable<Media> medias { get; set; }
        public IEnumerable<ProductSpecification> productSpecifications { get; set; }      
        public string productTypeName { get; set; }
        public IEnumerable<Review> reviews { get; set; }
        //Freq used for shop orderby
        public int frequency { get; set; }
        public bool isAllowReview { get; set; }
        public bool freeShipping { get; set; }
        public bool freeTax { get; set; }
        //Meta Tag
        public string metaTagDescription { get; set; }
        public string metaTagTitle { get; set; }
        public MetaTagType metaTagType { get; set; }
        public string metaTagImage { get; set; }
        public string metaTagImageAlt { get; set; }
        public string metaTagUrl { get; set; }
        public string metaTagLocale { get; set; }
        public string metaTagSitename { get; set; }
        public string metaTagVideo { get; set; }
        public string metaTagKeywords { get; set; }
    }
}
