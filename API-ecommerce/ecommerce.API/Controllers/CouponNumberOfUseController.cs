using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.Models;
using ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponNumberOfUseController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public CouponNumberOfUseController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CouponNumberOfUse couponNumberOfUse)
        {
            POJO model = new POJO();
            try
            {
                if (couponNumberOfUse == null)
                {
                    model.flag = false;
                    model.message = StaticValues.EmptyAPIRequest;
                    return BadRequest(model);
                }

                model = await unitOfWork.CouponNumberOfUse.Save(couponNumberOfUse);

                if (model == null)
                {
                    model.flag = false;
                    model.message = StaticValues.ErrorResponseAPIDB;
                    return NotFound(model);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = $"{StaticValues.ExceptionResponseAPI} {ex.ToString()}";
                return BadRequest(model);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest(StaticValues.EmptyAPIRequest);
                }

                CouponNumberOfUse model = await unitOfWork.CouponNumberOfUse.Get(id);

                if (model == null)
                {
                    return NotFound(StaticValues.ErrorResponseAPIDB);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"{StaticValues.ExceptionResponseAPI} {ex.ToString()}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IQueryable<CouponNumberOfUse> model = await unitOfWork.CouponNumberOfUse.GetAll();
                if (model == null)
                {
                    return NotFound(StaticValues.ErrorResponseAPIDB);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"{StaticValues.ExceptionResponseAPI} {ex.ToString()}");
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            POJO model = new POJO();
            try
            {
                if (id == null)
                {
                    model.flag = false;
                    model.message = StaticValues.EmptyAPIRequest;
                    return BadRequest(model);
                }

                model = await unitOfWork.CouponNumberOfUse.Delete(id);

                if (model == null)
                {
                    model.flag = false;
                    model.message = StaticValues.ErrorResponseAPIDB;
                    return NotFound(model);
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = $"{StaticValues.ExceptionResponseAPI} {ex.ToString()}";
                return BadRequest(model);
            }
        }
        [HttpGet("GetByCouponId/{id}")]
        public async Task<IActionResult> GetByCouponId(string id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest(StaticValues.EmptyAPIRequest);
                }
                var model = await unitOfWork.CouponNumberOfUse.GetAll(x => x.couponId.Equals(id));

                if (model is null)
                {
                    return NotFound(StaticValues.ErrorResponseAPIDB);
                }
                return Ok(model?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"{StaticValues.ExceptionResponseAPI} {ex.ToString()}");
            }
        }
    }
}