using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks; 
using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public ReportController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }

        //Get Monthly order for a year // paid orders only
        [HttpGet("GetOrderCountMonthly/{id}")]
        public async Task<IActionResult> GetOrderCountMonthly(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var models = await unitOfWork.Order.GetAll(x => x.createDate.Year.Equals(Convert.ToInt32(id)) && x.isPaid.Equals(true));

            if (models == null)
            {
                return NotFound();
            }

            var groupByData = models.GroupBy(p => p.createDate.Month).Select(g => new { key = g.Key, count = g.Count() }).ToList();
            MVReportNumberOfOrdersMonthly mVReportNumberOfOrdersMonthly = new MVReportNumberOfOrdersMonthly();

            foreach (var item in groupByData)
            {
                string comma = item == groupByData.Last() ? "" : ", ";
                mVReportNumberOfOrdersMonthly.month += $"\"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.key).ToString()}\"{comma}";
                mVReportNumberOfOrdersMonthly.count += $"{item.count.ToString()}{comma}";
            }

            return Ok(mVReportNumberOfOrdersMonthly);
        }

        //Get Monthly order, etcc for a year 
        [HttpGet("GetMonthlyCount/{id}")]
        public async Task<IActionResult> GetMonthlyCount(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var earningsAnnual = await unitOfWork.PaymentTransaction.GetAll(x => x.createDate.Year.Equals(Convert.ToInt32(id)));
            var pendingReviews = await unitOfWork.Review.GetAll(x => x.createDate.Year.Equals(Convert.ToInt32(id)) && x.isApproved.Equals(false) && x.isDeleted.Equals(false));
            var products = await unitOfWork.Product.GetAll(x => x.createDate.Year.Equals(Convert.ToInt32(id)) && x.isDeleted.Equals(false));
            var pendingShippment = await unitOfWork.TrackingOrder.GetAll(x => x.trackingStatus != TrackingStatus.Deleted || x.trackingStatus != TrackingStatus.Delivered);
            var pendingReturnOrder = await unitOfWork.OrderReturnCase.GetAll(x => x.status.Equals(Status.Open) || x.status.Equals(Status.Pending));
            var pendingCompliment = await unitOfWork.OrderComplaintCase.GetAll(x => x.status.Equals(Status.Open) || x.status.Equals(Status.Pending));




            MVReportNumberOfProductReview mVReportNumberOfProductReview = new MVReportNumberOfProductReview()
            {
                earningsAnnual = earningsAnnual.Sum(x => x.amount),
                pendingProductsCount = products.Count(x => x.isApproved.Equals(false)),
                productsCount = products.Count(x => x.isApproved.Equals(true)),
                pendingReviewsCount = pendingReviews.Count(),
                pendingShippment = pendingShippment.Count(),
                pendingReturnOrder = pendingReturnOrder.Count(),
                pendingCompliment = pendingCompliment.Count()
            }; 
            return Ok(mVReportNumberOfProductReview);
        }

        //Get Monthly order, etcc for a year 
        [HttpGet("GetPaymentMethodNumber/{id}")]
        public async Task<IActionResult> GetPaymentMethodNumber(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var orders = await unitOfWork.Order.GetAll(x => x.createDate.Year.Equals(Convert.ToInt32(id)) && x.isPaid.Equals(true));


            MVReportNumberOfPaymentMethod mVReportNumberOfPaymentMethod = new MVReportNumberOfPaymentMethod()
            {
              card = orders.Count(x => x.paymentMethod.Equals(PaymentMethod.Card)),
              cash = orders.Count(x => x.paymentMethod.Equals(PaymentMethod.Cash))
            };
            return Ok(mVReportNumberOfPaymentMethod);
        }

        [HttpGet("GetOrderByUserId/{id}")]
        public async Task<IActionResult> GetOrderByUserId(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            IEnumerable<Order> model = await unitOfWork.Order.GetAll(x => x.createdBy.Equals(id) && x.isDeleted.Equals(false));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.OrderByDescending(x => x.createDate));
        }

        [HttpGet("GetUserCases/{id}")]
        public async Task<IActionResult> GetUserCases(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            MVReportUserCases mVReportUserCases = new MVReportUserCases()
            {
                OrderComplaintCases = await unitOfWork.OrderComplaintCase.GetAll(x => x.createdBy.Equals(id)),
                OrderReturnCases = await unitOfWork.OrderReturnCase.GetAll(x => x.createdBy.Equals(id))
            };

            if (mVReportUserCases == null)
            {
                return NotFound();
            }
            return Ok(mVReportUserCases);
        }

        [HttpGet("GetUserOpenCasesByOrderId/{id}")]
        public async Task<IActionResult> GetUserOpenCasesByOrderId(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            MVReportUserCases mVReportUserCases = new MVReportUserCases()
            {
                OrderComplaintCases = await unitOfWork.OrderComplaintCase.GetAll(x => x.orderId.Equals(id) && x.status.Equals(Status.Open)),
                OrderReturnCases = await unitOfWork.OrderReturnCase.GetAll(x => x.orderId.Equals(id) && x.status.Equals(Status.Open))
            };

            if (mVReportUserCases == null)
            {
                return NotFound();
            }
            return Ok(mVReportUserCases);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Order> model = await unitOfWork.Order.GetAll(x => x.isDeleted.Equals(false));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

      

         
    }
}