using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public ReviewController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Review review)
        {
            POJO model = new POJO();

            if (review == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Review.Save(review);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            //Update the rating of product, after accept the review only.....
            if (model.flag && review.isApproved)
            {
                //Get the review to take the product Id.
                Review _review = await unitOfWork.Review.Get(review.id);
                //get the product
                Product product = await unitOfWork.Product.Get(_review.productId);
                if (product != null)
                {
                    product.rating = ((product.rating + _review.rating) / (product.rating.Equals(0) ? 1 : 2));
                    model = await unitOfWork.Product.Save(product);
                }
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
            Review model = await unitOfWork.Review.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Review> model = await unitOfWork.Review.GetAll(x => x.isDeleted.Equals(false) && x.isApproved.Equals(true));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetProductReviews/{id}")]
        public async Task<IActionResult> GetProductReviews(string id)
        {
            IQueryable<Review> model = await unitOfWork.Review.GetAll(x => x.productId.Equals(Convert.ToInt32(id)) && x.isApproved.Equals(true) && x.isDeleted.Equals(false));
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

            model = await unitOfWork.Review.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] Review review)
        {
            POJO model = new POJO();

            if (review == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Review.UpdateIsDelete(review);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("GetReviewsNeedApprove")]
        public async Task<IActionResult> GetReviewsNeedApprove()
        {
            IQueryable<Review> model = await unitOfWork.Review.GetAll(x => x.isApproved.Equals(false) && x.isDeleted.Equals(false));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        // Check if the product is bought by the customer and not reviewd.
        [HttpPost("CheckCustomerIsReview")]
        public async Task<IActionResult> CheckCustomerReview([FromBody] Review review)
        {
            try
            {
                POJO model = new POJO();
                IQueryable<Review> reviews = await unitOfWork.Review.GetAll(x => x.createdBy.Equals(review.createdBy) &&
                                                                                 x.productId.Equals(review.productId));

                IQueryable<OrderLine> orderLines = await unitOfWork.OrderLine.GetAll(x => x.modifiedBy.Equals(review.createdBy) &&
                                                                                          x.productId.Equals(review.productId));
                model.flag = false;
                model.message = "Customer Free to review.";
                model.id = review.productId.ToString();
                if (reviews.Any() || !orderLines.Any())
                {
                    //We are passing the productId to load the reivews in the partial
                    model.id = review.productId.ToString();
                    model.flag = true;
                    model.message = $"The customer can't review this product coz reviews = {reviews.Any()}, orderLines = {orderLines.Any()}";
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

    }
}