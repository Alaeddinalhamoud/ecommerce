using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public PaymentTransactionController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentTransaction paymentTransaction)
        {
            POJO model = new POJO();

            if (paymentTransaction == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
            model = await unitOfWork.PaymentTransaction.Save(paymentTransaction);

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
            IQueryable<PaymentTransaction> model = await unitOfWork.PaymentTransaction.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetPaymentTransactionByOrder/{id}")]
        public async Task<IActionResult> GetPaymentTransactionByOrder(string id)
        {
            //Id is the status true or false
            IQueryable<PaymentTransaction> paymentTransaction = await unitOfWork.PaymentTransaction.GetAll(x => x.orderId.Equals(id));
            return Ok(paymentTransaction?.FirstOrDefault());
        }
    }
}