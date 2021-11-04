using System;
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
    public class MostViewedProductController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public MostViewedProductController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MostViewedProduct mostViewedProduct)
        {
            POJO model = new POJO();

            if (mostViewedProduct == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
            model = await unitOfWork.MostViewedProduct.Save(mostViewedProduct);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IQueryable<MostViewedProduct> mostViewedProducts = await unitOfWork.MostViewedProduct.GetAll(x => x.lastVisitDate <= DateTime.Now.AddMonths(1));
                var medias = await unitOfWork.Media.GetAll(m => m.isDeleted.Equals(false));
                var products = await unitOfWork.Product.GetAll(p => p.isDeleted.Equals(false) && p.isApproved.Equals(true));

                IEnumerable<ProductDetail> models = from mostViewedProduct in mostViewedProducts
                                                    join product in products on mostViewedProduct.productId equals product.id
                                                    select new ProductDetail
                                                    {
                                                        name = product.name,
                                                        oldPrice = product.oldPrice,
                                                        price = product.price,
                                                        medias = medias.Where(c => c.productId.Equals(product.id)),
                                                        id = product.id,
                                                        rating = product.rating,
                                                        qty = product.qty
                                                    };
                if (models == null)
                {
                    return NotFound();
                }
                return Ok(models.Take(20));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            } 
        }
    }
}