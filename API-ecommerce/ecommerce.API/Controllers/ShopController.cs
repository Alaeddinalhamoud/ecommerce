using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data.MVData;
using ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ShopController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        } 
       
        [HttpGet("ShopCategories/{id}")]
        public async Task<IActionResult> GetShopCategories(string id)
        {
            ShopCategories model = new ShopCategories()
            {
                categories = await unitOfWork.Category.GetAll(x => x.isDeleted.Equals(false)),
                brands = await unitOfWork.Brand.GetAll(x => x.isDeleted.Equals(false)),
                productTypes = await unitOfWork.ProductType.GetAll(x => x.isDeleted.Equals(false))
            };

            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetShop")]
        public async Task<IActionResult> GetShop()
        {
            return Ok(await GetShopDetails());
        }

        private async Task<IQueryable<ProductDetail>> GetShopDetails()
        {
            var products = await unitOfWork.Product.GetAll(x => x.isApproved.Equals(true) && x.isDeleted.Equals(false));
            var categories = await unitOfWork.Category.GetAll();
            var freq = await unitOfWork.MostViewedProduct.GetAll();
            //var brands = await unitOfWork.Brand.GetAll();
            //var productTypes = await unitOfWork.ProductType.GetAll();
            //we need to pass the filter 
            IQueryable<ProductDetail> models = from product in products
                                               join category in categories on product.categoryId equals category.id

                                               select new ProductDetail
                                               {
                                                   id = product.id,
                                                   name = product.name,
                                                   createdBy = product.createdBy,
                                                   vendorId = product.vendorId,
                                                   isDeleted = product.isDeleted,
                                                   oldPrice = product.oldPrice,
                                                   price = product.price,
                                                   categoryId = product.categoryId,
                                                   description = product.description,
                                                   tags = product.tags,
                                                   isApproved = product.isApproved,
                                                   videoUrl = product.videoUrl,
                                                   rating = product.rating,
                                                   placeOfOrigin = product.placeOfOrigin,
                                                   modelNumber = product.modelNumber,
                                                   expiryDate = product.expiryDate,
                                                   barcode = product.barcode,
                                                   inventoryCode = product.inventoryCode,
                                                   LotNumber = product.LotNumber,
                                                   brandId = product.brandId,
                                                   productTypeId = product.productTypeId,
                                                   createDate = product.createDate,
                                                   updateDate = product.updateDate,
                                                   modifiedBy = product.modifiedBy,
                                                   //frequency = freq.FirstOrDefault(x => x.productId.Equals(product.id)).frequency,
                                                   categoryName = category.name,
                                                   shortDescription = product.shortDescription,
                                                   qty = product.qty
                                               };

            List<ProductDetail> productsDetails = new List<ProductDetail>();
            foreach (var item in models)
            {
                int? ProductFrequency = freq?.FirstOrDefault(x => x.productId.Equals(item.id))?.frequency;
                ProductDetail productDetail = new ProductDetail()
                {
                    id = item.id,
                    name = item.name,
                    rating = item.rating,
                    medias = await unitOfWork.Media.GetAll(x => x.productId.Equals(item.id) && x.isDeleted.Equals(false)),
                    oldPrice = item.oldPrice,
                    price = item.price,
                    createDate = item.createDate,
                    categoryName = item.categoryName,
                    categoryId = item.categoryId,
                    brandId = item.brandId,
                    productTypeId = item.productTypeId,
                    shortDescription = item.shortDescription,
                    frequency = ProductFrequency.Equals(null) ? 0 : Convert.ToInt32(ProductFrequency),
                    qty = item.qty
                };
                productsDetails.Add(productDetail);
            }
            return productsDetails.AsQueryable();
        }

    }
}