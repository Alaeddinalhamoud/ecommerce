using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecentlyViewedProductController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public RecentlyViewedProductController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RecentlyViewedProduct recentlyViewedProduct)
        {
            POJO model = new POJO();

            if (recentlyViewedProduct == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
            model = await unitOfWork.RecentlyViewedProduct.Save(recentlyViewedProduct);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }
        //RecentlyViewedProduct by userID
        [HttpGet("GetRecentlyViewedProducts/{id}")]
        public async Task<IActionResult> GetProductReviews(string id)
        {
            IQueryable<RecentlyViewedProduct> recentlyViewedProducts = await unitOfWork.RecentlyViewedProduct.GetAll(x => x.userId.Equals(id));          
            var medias = await unitOfWork.Media.GetAll(m => m.isDeleted.Equals(false));
            var products = await unitOfWork.Product.GetAll(p => p.isDeleted.Equals(false) && p.isApproved.Equals(true));

            IQueryable<RecentlyViewedProductDetail> models = from recentlyViewedProduct in recentlyViewedProducts
                                                             join product in products on recentlyViewedProduct.productId equals product.id

                                                 select new RecentlyViewedProductDetail
                                                 {
                                                     id = recentlyViewedProduct.id,
                                                     productName = product.name,
                                                     oldPrice = product.oldPrice,
                                                     price = product.price,
                                                     productImage = medias.Where(c => c.productId.Equals(product.id)).FirstOrDefault().path,                                                   
                                                     productId = product.id,
                                                     userId = id,
                                                     rating = product.rating,
                                                     qty = product.qty
                                                 };
            if (models == null)
            {
                return NotFound();
            }
            return Ok(models);
        }
    }
}