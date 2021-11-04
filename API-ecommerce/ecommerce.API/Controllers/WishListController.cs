using System.Collections.Generic;
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
    public class WishListController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public WishListController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Wishlist wishlist)
        {
            POJO model = new POJO();

            if (wishlist == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.WishList.Save(wishlist);

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
            Wishlist model = await unitOfWork.WishList.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async  Task<IActionResult> GetAll()
        {
            IQueryable<Wishlist> model = await unitOfWork.WishList.GetAll();
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

            model = await unitOfWork.WishList.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

         
        //Get the number of items in wishlist
        [HttpGet("NumberOfProducts/{id}")]
        public async Task<IActionResult> NumberOfProducts(string id)
        {
            Wishlist wishlistNumbers = new Wishlist();
            if (id == null)
            {
                wishlistNumbers.id = 0;
                return Ok(wishlistNumbers);
            }
            var model = await unitOfWork.WishList.GetAll(x => x.userId.Equals(id));
            //We are useing Id just to pass the number of products.
            //rather than create new model 
            wishlistNumbers.id = model.Count(); 
            return Ok(wishlistNumbers);
        }

        //Get the number of items in wishlist
        [HttpPost("wishlistStatus")]
        public async Task<IActionResult> wishlistStatus([FromBody] Wishlist wishlist)
        {  
            if (wishlist == null)
            { 
                return NotFound();
            }
            var model = await unitOfWork.WishList.GetAll(x => x.userId.Equals(wishlist.userId) && x.productId.Equals(wishlist.productId));
           
            POJO status = new POJO()
            {
                flag = model.Count().Equals(0) ? false : true 
             };
            return Ok(status);
        }

        [HttpGet("GetMyWishlist/{id}")]
        public async Task<IActionResult> GetMyWishlist(string id)
        {
            var wishlists = await unitOfWork.WishList.GetAll(w => w.userId.Equals(id));            
            var medias = await unitOfWork.Media.GetAll(m => m.isDeleted.Equals(false));            
            var products = await unitOfWork.Product.GetAll(p => p.isDeleted.Equals(false) && p.isApproved.Equals(true));

            IQueryable<WishlistProduct> models = from wishlist in wishlists
                                                 join product in products on wishlist.productId equals product.id
                                               
                                               select new WishlistProduct
                                               {
                                                   id = wishlist.id,
                                                   productName = product.name,
                                                   price = product.price,
                                                   productImage = medias.Where(c => c.productId.Equals(product.id)).FirstOrDefault().path,
                                                   qty = product.qty,
                                                   productId = product.id
                                               };  
            return Ok(models);
        }

    }
}