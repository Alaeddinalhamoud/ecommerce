using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.Services;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IService<User> serviceUser;
        private readonly string url = "/api/User/";

        public ProductController(IUnitOfWork _UnitOfWork, IService<User> _serviceUser)
        {
            unitOfWork = _UnitOfWork;
            serviceUser = _serviceUser;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            POJO model = new POJO();

            if (product == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
            model = await unitOfWork.Product.Save(product);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Product model = await unitOfWork.Product.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //we need to pass the filter
            //  IQueryable<Product> model = unitOfWork.Product.GetAll().Where(x => x.isDeleted.Equals(false));
            IQueryable<Product> model = await unitOfWork.Product.GetAll(x => x.isDeleted.Equals(false) && x.isApproved.Equals(true));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        //Retuen the deleted products, does not matter is approved or no
        [HttpGet("GetDeletedProducts")]
        public async Task<IActionResult> GetDeletedProducts()
        {
            //we need to pass the filter
            //  IQueryable<Product> model = unitOfWork.Product.GetAll().Where(x => x.isDeleted.Equals(false));
            IQueryable<Product> model = await unitOfWork.Product.GetAll(x => x.isDeleted.Equals(true));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        //[HttpGet("GetProductDetails")]
        //public IActionResult GetProductDetails()
        //{
        //    //we need to pass the filter
        //    IQueryable<Product> model = unitOfWork.Product.GetAll(includeProperties: "Category");
        //    if (model == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(model);
        //} 

        [HttpGet("GetProductDetailById/{id}")]
        public async Task<IActionResult> GetProductDetailById(int id)
        {
            //we need to pass the filter 
            //We have to pass all the items even the deletd.... 
            var products = await unitOfWork.Product.GetAll(x => x.id.Equals(id));
           

            if (products.Count() > 0)
            {
                var medias = await unitOfWork.Media.GetAll(med => med.productId.Equals(id) && med.isDeleted.Equals(false));
                var productSpecifications = await unitOfWork.ProductSpecification.GetAll(ps => ps.productId.Equals(id) && ps.isDeleted.Equals(false));
                var reviews = await unitOfWork.Review.GetAll(rv => rv.productId.Equals(id) && rv.isDeleted.Equals(false) && rv.isApproved.Equals(true));
                var metaTag = await unitOfWork.MetaTag.GetAll(x => x.productId.Equals(id));

                var brands = await unitOfWork.Brand.GetAll();
                var categories = await unitOfWork.Category.GetAll();
                var productTypes = await unitOfWork.ProductType.GetAll();

                IQueryable<ProductDetail> models = from product in products
                                                   join brand in brands on product.brandId equals brand.id
                                                   join category in categories on product.categoryId equals category.id
                                                   join productType in productTypes on product.productTypeId equals productType.id
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
                                                       brandName = brand.name,
                                                       categoryName = category.name,
                                                       medias = medias,
                                                       productSpecifications = productSpecifications,
                                                       productTypeName = productType.name,
                                                       reviews = reviews,
                                                     //  frequency = product.frequency,
                                                       qty = product.qty,
                                                       packageType = product.packageType,
                                                       shortDescription = product.shortDescription,
                                                       freeShipping = product.freeShipping,
                                                       freeTax = product.freeTax
                                                   };
                if (models == null)
                {
                    return NotFound();
                }
                var model = models.FirstOrDefault();
                //add meta tags
                if (metaTag != null && metaTag.Count() > 0)
                {
                    var meta = metaTag.FirstOrDefault();
                    model.metaTagDescription = meta.description;
                    model.metaTagTitle = meta.title;
                    model.metaTagType = meta.metaTagType;
                    model.metaTagImage = meta.image;
                    model.metaTagImageAlt = meta.imageAlt;
                    model.metaTagUrl = meta.url;
                    model.metaTagVideo = meta.video;
                    model.metaTagSitename = meta.sitename;
                    model.metaTagKeywords = meta.keywords;
                    model.metaTagLocale = meta.locale;
                }
                //TO get the user details
                try
                {
                    var token = await serviceUser.GetAccessToken();
                    var createdByUser = await serviceUser.Get(model.createdBy, url, token);
                    model.createdBy = createdByUser.name;
                    var modifiedByUser = await serviceUser.Get(model.modifiedBy, url, token);
                    model.modifiedBy = modifiedByUser.name;
                    var vendorName = await serviceUser.Get(model.vendorId, url, token);
                    model.vendorId = vendorName.name; 
                }
                catch
                {
                    //In case if the user not exist 
                    model.createdBy = "Deleted User";
                    model.modifiedBy = "Deleted User";
                    model.vendorId = "Deleted User";
                }
                return Ok(model);
            }

            return BadRequest();
        }

        [HttpPost("GetAllByPost")]
        public IActionResult GetAllByPost(DataFiltter<Product> dataFiltter)
        {
            IQueryable<Product> model = unitOfWork.Product.GetAllByPost(dataFiltter);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            POJO model = new POJO();

            if (id == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Product.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] Product product)
        {
            POJO model = new POJO();

            if (product == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Product.UpdateIsDelete(product);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        //This func will take last 20 related produc by category
        [HttpGet("RelatedProduts/{id}")]
        public async Task<IActionResult> RelatedProduts(int id)
        {
            var products = await unitOfWork.Product.GetAll(x => x.categoryId.Equals(id) && x.isDeleted.Equals(false) && x.isApproved.Equals(true));
            List<ProductDetail> productsDetails = await GetProductDetails(products); 
            return Ok(productsDetails.AsQueryable());
        }  

        [HttpGet("GetProductsNeedApprove")]
        public async Task<IActionResult> GetProductsNeedApprove()
        {
            IQueryable<Product> model = await unitOfWork.Product.GetAll(x => x.isApproved.Equals(false) && x.isDeleted.Equals(false));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }


        //This func will take last 20 days new products 
        [HttpGet("NewArrivalProducts")]
        public async Task<IActionResult> NewArrivalProducts()
        {
            var products = await unitOfWork.Product.GetAll(x => x.createDate >= DateTime.Now.AddDays(-20) && x.isDeleted.Equals(false) && x.isApproved.Equals(true));
            List<ProductDetail> productsDetails = await GetProductDetails(products);
            return Ok(productsDetails);
        }

        private async Task<List<ProductDetail>> GetProductDetails(IQueryable<Product> products)
        {
            var brands = await unitOfWork.Brand.GetAll();
            var categories = await unitOfWork.Category.GetAll();
            var productTypes = await unitOfWork.ProductType.GetAll();
            //we need to pass the filter 
            IQueryable<ProductDetail> models = from product in products.Take(20)
                                               join brand in brands on product.brandId equals brand.id
                                               join category in categories on product.categoryId equals category.id
                                               join productType in productTypes on product.productTypeId equals productType.id
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
                                                   brandName = brand.name,
                                                   categoryName = category.name,
                                                   productTypeName = productType.name,
                                                  // frequency = product.frequency,
                                                   qty = product.qty,
                                                   packageType = product.packageType,
                                                   freeShipping = product.freeShipping,
                                                   freeTax = product.freeTax
                                               };

            List<ProductDetail> productsDetails = new List<ProductDetail>();
            foreach (var item in models)
            {
                ProductDetail productDetail = new ProductDetail()
                {
                    id = item.id,
                    name = item.name,
                    categoryName = item.categoryName,
                    rating = item.rating,
                    medias = await unitOfWork.Media.GetAll(x => x.productId.Equals(item.id) && x.isDeleted.Equals(false)),
                    oldPrice = item.oldPrice,
                    price = item.price,
                    qty = item.qty,
                    freeShipping = item.freeShipping,
                    freeTax = item.freeTax
                };
                productsDetails.Add(productDetail);
            }
            return productsDetails;
        }


        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }

    }
}