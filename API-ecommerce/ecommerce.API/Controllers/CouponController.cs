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
    public class CouponController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public CouponController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Coupon coupon)
        {
            POJO model = new POJO();
            try
            {
                if (coupon == null)
                {
                    model.flag = false;
                    model.message = StaticValues.EmptyAPIRequest;
                    return BadRequest(model);
                }

                model = await unitOfWork.Coupon.Save(coupon);

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

                Coupon model = await unitOfWork.Coupon.Get(id);

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
                IQueryable<Coupon> model = await unitOfWork.Coupon.GetAll();
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

                model = await unitOfWork.Coupon.Delete(id);

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

        [HttpGet("GetByCode/{id}")]
        public async Task<IActionResult> GetByCode(string id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest(StaticValues.EmptyAPIRequest);
                }
                var model = await unitOfWork.Coupon.GetAll(x => x.code.Equals(id));

                if (model == null)
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