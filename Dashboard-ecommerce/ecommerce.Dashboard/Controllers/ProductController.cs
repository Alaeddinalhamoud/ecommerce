using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Dashboard.Models;
using ecommerce.Data;
using ecommerce.Data.MVData;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using UtilityFunctions.Data;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {

        //Calling product services
        private readonly IService<Product> service;
        private readonly IService<ProductDetail> serviceDetails;
        private readonly string url = "/api/product/";
        //Calling Producttype to fill the add new product drowpdownlsit
        private readonly IService<ProductType> serviceProductType;
        private readonly string urlProductType = "/api/producttype/";
        //Calling Category Service to do same as producttype
        private readonly IService<Category> serviceCategory;
        private readonly string urlCategory = "/api/category/";
        //Calling Brand service to fill dropdownlist
        private readonly IService<Brand> serviceBrand;
        private readonly string urlBrand = "/api/brand/";
        private readonly ILogger<ProductController> _logger;

        //Temp Message
        [TempData]
        public string StatusMessage { get; set; }

        public ProductController(IService<Product> _service,
            IService<ProductType> _serviceProductType,
            IService<Category> _serviceCategory,
            IService<Brand> _serviceBrand,
            IService<ProductDetail> _serviceDetails,
            ILogger<ProductController> logger)
        {
            service = _service;
            serviceProductType = _serviceProductType;
            serviceCategory = _serviceCategory;
            serviceBrand = _serviceBrand;
            serviceDetails = _serviceDetails;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable<Product> products = await service.GetAll(url, await GetToken());
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
        //Display the deleted product in the recyle bin
        public async Task<IActionResult> GetDeletedProducts()
        {
            try
            {
                IQueryable<Product> products = await service.GetAll($"{url}GetDeletedProducts", await GetToken());
                return View("Index", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Save()
        {
            try
            {
                MvProduct product = new MvProduct()
                {
                    id = 0,
                    productTypes = new SelectList(await serviceProductType.GetAll(urlProductType, await GetToken()), "id", "name"),
                    categories = new SelectList(await serviceCategory.GetAll(urlCategory, await GetToken()), "id", "name"),
                    brands = new SelectList(await serviceBrand.GetAll(urlBrand, await GetToken()), "id", "name"),
                    countries = new SelectList(Array.AsReadOnly((Countries[])Enum.GetValues(typeof(Countries))).ToList())
                };
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(MvProduct mVProduct)
        {
            POJO model = new POJO();
            mVProduct.productTypes = new SelectList(await serviceProductType.GetAll(urlProductType, await GetToken()), "id", "name");
            mVProduct.categories = new SelectList(await serviceCategory.GetAll(urlCategory, await GetToken()), "id", "name");
            mVProduct.brands = new SelectList(await serviceBrand.GetAll(urlBrand, await GetToken()), "id", "name");
            mVProduct.countries = new SelectList(Array.AsReadOnly((Countries[])Enum.GetValues(typeof(Countries))).ToList());
            if (ModelState.IsValid)
            {
                try
                {
                    Product product = new Product()
                    {
                        id = mVProduct.id,
                        name = mVProduct.name,
                        oldPrice = mVProduct.oldPrice,
                        price = mVProduct.price,
                        categoryId = mVProduct.categoryId,
                        description = mVProduct.description,
                        tags = mVProduct.tags,
                        videoUrl = mVProduct.videoUrl,
                        placeOfOrigin = mVProduct.placeOfOrigin,
                        modelNumber = mVProduct.modelNumber,
                        expiryDate = mVProduct.expiryDate,
                        barcode = mVProduct.barcode,
                        inventoryCode = mVProduct.inventoryCode,
                        LotNumber = mVProduct.LotNumber,
                        brandId = mVProduct.brandId,
                        productTypeId = mVProduct.productTypeId,
                        isApproved = false,
                        isDeleted = false,
                        vendorId = mVProduct.id.Equals(0) ? GetCurrentUserId() : null, //Vedor Id is the user who add the product (owner)
                        createdBy = mVProduct.id.Equals(0) ? GetCurrentUserId() : null, // Could be changed by update or edit ( the editor)
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        modifiedBy = GetCurrentUserId(),
                        qty = mVProduct.qty,
                        shortDescription = mVProduct.shortDescription,
                        packageType = mVProduct.packageType,
                        freeShipping = mVProduct.freeShipping,
                        freeTax = mVProduct.freeTax
                    };
                    model = await service.Post(product, url, await GetToken());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                //Take you to add image and files
                if (model.flag && mVProduct.updateImage)
                {
                    return RedirectToAction("MultiUpload", "Media", new FileUploaderRequest() { id = Convert.ToInt32(model.id), sourceController = "Product" });

                }
                //Take you to add Product Specification
                if (model.flag && !mVProduct.updateImage)
                {
                    return RedirectToAction("Save", "ProductSpecification", new { id = model.id });
                }
                if (!model.flag)
                {
                    StatusMessage = $"{model.message.ToString()}";
                    ModelState.AddModelError("", model.message);
                    return View(mVProduct);
                }

                //Need to check it too.
                ModelState.AddModelError("", model.message);
            }

            return View(mVProduct);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, bool value = false)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            Product product = new Product()
            {
                id = id,
                isDeleted = value,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            {
                POJO model = await service.Post(product, $"{url}UpdateIsDeleted", await GetToken());
                var status = value.Equals(true) ? "Deleted" : "Restored";
                StatusMessage = $"Product Id {id} has been {status}.";
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                Product product = await service.Get(id, url, await GetToken());
                MvProduct mVProduct = new MvProduct()
                {
                    id = product.id,
                    name = product.name,
                    oldPrice = product.oldPrice,
                    price = product.price,
                    categoryId = product.categoryId,
                    description = product.description,
                    tags = product.tags,
                    videoUrl = product.videoUrl,
                    placeOfOrigin = product.placeOfOrigin,
                    modelNumber = product.modelNumber,
                    expiryDate = product.expiryDate,
                    barcode = product.barcode,
                    inventoryCode = product.inventoryCode,
                    LotNumber = product.LotNumber,
                    brandId = product.brandId,
                    productTypeId = product.productTypeId,
                    qty = product.qty,
                    packageType = product.packageType,
                    shortDescription = product.shortDescription,
                    freeShipping = product.freeShipping,
                    freeTax = product.freeTax,
                    productTypes = new SelectList(await serviceProductType.GetAll(urlProductType, await GetToken()), "id", "name"),
                    categories = new SelectList(await serviceCategory.GetAll(urlCategory, await GetToken()), "id", "name"),
                    brands = new SelectList(await serviceBrand.GetAll(urlBrand, await GetToken()), "id", "name"),
                    countries = new SelectList(Array.AsReadOnly((Countries[])Enum.GetValues(typeof(Countries))).ToList())
                };
                return View("Save", mVProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {

                ProductDetail productDetail = await serviceDetails.Get(id, $"{url}GetProductDetailById/", await GetToken());
                return View(productDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }
        //Get only product needs apprive
        public async Task<IActionResult> GetProductsNeedApprove()
        {
            try
            {
                IQueryable<Product> products = await service.GetAll($"{url}GetProductsNeedApprove", await GetToken());
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> ProductApprove(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            {
                Product product = await service.Get(id.ToString(), url, await GetToken());
                if (product == null)
                {
                    throw new Exception($"Empty product {id}");
                }
                product.isApproved = true;
                product.modifiedBy = GetCurrentUserId();

                POJO model = await service.Post(product, url, await GetToken());
                StatusMessage = $"Product Id {id} has been aproved.";
                return RedirectToAction("GetProductsNeedApprove", "Product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

        }

        public string GetCurrentUserId()
        {
            return User?.FindFirst(c => c.Type == "sub")?.Value;
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}