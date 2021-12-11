using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.FrontEnd.Models;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ecommerce.FrontEnd.Controllers
{
    public class ShopController : Controller
    {
        private readonly IService<ProductDetail> serviceDetails;
        private readonly string url = "/api/shop/";
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/Setting/";
        private readonly ILogger<ShopController> _logger;

        public ShopController(IService<ProductDetail> _serviceDetails, IService<Setting> _serviceSetting, ILogger<ShopController> logger)
        {
            serviceDetails = _serviceDetails;
            serviceSetting = _serviceSetting;
            _logger = logger;
        }

        [Route("shop")]
        [Route("product")]       
        public async Task<IActionResult> Index(int currentPage, int? categoryId,
                                              string sortOrder = null, string categoryFiltter = null,
                                              string searchBox = null)
        {
            try
            {
                var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
                int pageSize = settings?.FirstOrDefault()?.pageSize ?? 5;
                var data = await GetShopFilter(currentPage, categoryId, sortOrder, categoryFiltter, searchBox);
                PaginationModel paginationModel = new PaginationModel();
                paginationModel.Count = data.Count();
                paginationModel.CurrentPage = currentPage;
                paginationModel.PageSize = pageSize;
                paginationModel.Data = data.OrderBy(d => d.createDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                return View(paginationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Shop");
                return RedirectToAction("Error", "Home");
            }
        }
        //Return PartialView
        [HttpGet]
        public async Task<IActionResult> page(int currentPage, int? categoryId,
                                              string sortOrder = null, string categoryFiltter = null,
                                              string searchBox = null)
        {
            try
            {
                var settings = await serviceSetting.GetAll(urlSetting, await GetToken());
                int pageSize = settings?.FirstOrDefault()?.pageSize ?? 5;

                var data = await GetShopFilter(currentPage, categoryId, sortOrder, categoryFiltter, searchBox);
                PaginationModel paginationModel = new PaginationModel();
                paginationModel.Count = data.Count();
                paginationModel.CurrentPage = currentPage;
                paginationModel.PageSize = pageSize;

                paginationModel.Data = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                return PartialView("_ShopProducts", paginationModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Shop");
                return RedirectToAction("Error", "Home");
            }
        }


        private async Task<IQueryable<ProductDetail>> GetShopFilter(int? pageNumber, int? categoryId,
                                              string sortOrder = null, string categoryFiltter = null,
                                              string searchBox = null)
        {

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "1" : sortOrder;
            ViewData["CategoryId"] = categoryId;
            ViewData["CategoryFiltter"] = categoryFiltter;
            ViewData["CurrentFilter"] = searchBox;

            var data = await serviceDetails.GetAll($"{url}GetShop", await GetToken());

            if (!String.IsNullOrEmpty(searchBox))
            {
                data = data.Where(s => s.name.ToUpper().Contains(searchBox.ToUpper()));
            }

            switch (categoryFiltter)
            {
                case "Category":
                    data = data.Where(c => c.categoryId.Equals(categoryId));
                    break;
                case "Brand":
                    data = data.Where(c => c.brandId.Equals(categoryId));
                    break;
                case "ProductType":
                    data = data.Where(c => c.productTypeId.Equals(categoryId));
                    break;
            }

            switch (sortOrder)
            {
                case "2":
                    data = data.OrderByDescending(s => s.rating);
                    break;
                case "1":
                    data = data.OrderByDescending(s => s.frequency);
                    break;
                case "3":
                    data = data.OrderByDescending(s => s.createDate);
                    break;
                case "4":
                    data = data.OrderBy(s => s.price);
                    break;
                case "5":
                    data = data.OrderByDescending(s => s.price);
                    break;
                case "6":
                    data = data.OrderBy(s => s.name);
                    break;
                case "7":
                    data = data.OrderByDescending(s => s.name);
                    break;
                //Should be featured product by defult.
                default:
                    data = data.OrderByDescending(s => s.rating);
                    break;
            }
            return data;
        }

        private async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Medi_Access")))
            {
                HttpContext.Session.SetString("_Medi_Access", await serviceDetails.GetAccessToken());
            }
            return HttpContext.Session.GetString("_Medi_Access");
        }

    }
}