using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Data.Helper;
using ecommerce.Data.MVData;
using ecommerce.Services;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IService<User> serviceUser;
        private readonly string url = "/api/User/";
        public CartController(IUnitOfWork _UnitOfWork, IService<User> _serviceUser)
        {
            unitOfWork = _UnitOfWork;
            serviceUser = _serviceUser;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cart cart)
        {
            POJO model = new POJO();

            if (cart == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Cart.Save(cart);

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
            Cart model = await unitOfWork.Cart.Get(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Cart> model = await unitOfWork.Cart.GetAll(x => x.isDeleted.Equals(false));
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

            model = await unitOfWork.Cart.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpPost("UpdateIsDeleted")]
        public async Task<IActionResult> UpdateIsDeleted([FromBody] Cart cart)
        {
            POJO model = new POJO();

            if (cart == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Cart.UpdateIsDelete(cart);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        //Add To CartLine if there is not opened cart, create new one and sent the cartline.
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartLine cartLine)
        {
            POJO model = new POJO();
            try
            {
                if (cartLine == null)
                {
                    model.flag = false;
                    model.message = "Empty request.";
                    return Ok(model);
                }
                if(cartLine.qty.Equals(0) || cartLine.qty < 0)
                {
                    model = await unitOfWork.CartLine.Delete(cartLine.id);
                    model.flag = false;
                    model.message = "Cart line qty is 0";
                    return Ok(model);
                }
                //Get the product price
                Product product = await unitOfWork.Product.Get(cartLine.productId);
                if (product == null)
                {
                    model.flag = false;
                    model.message = "Prodcut Price issue, Empty response.";
                    return Ok(model);
                }
                //Check if the user has an open cart.
                var openedCarts = await unitOfWork.Cart.GetAll(x => x.createdBy.Equals(cartLine.createdBy));
                var openedCart = openedCarts?.FirstOrDefault(x => x.status.Equals(Status.Open));
                //If there is no open cart need to create new one
                if (openedCart == null)
                {

                    model = await unitOfWork.Cart.Save(new Cart()
                    {
                        createdBy = cartLine.createdBy,
                        createDate = DateTime.Now,
                        status = Status.Open
                    });

                    if (model == null)
                    {
                        model.flag = false;
                        model.message = "Empty table, Repo is sick.";
                        return Ok(model);
                    }

                    //Proccess to cart line (create cart line ) Cart total processed on the repo
                    cartLine.cartId = Convert.ToInt32(model.id);
                    cartLine.price = product.price;
                    cartLine.freeTax = product.freeTax;
                    cartLine.freeShipping = product.freeShipping;
                    POJO pOJOCartLine = await unitOfWork.CartLine.Save(cartLine);

                    if (pOJOCartLine == null)
                    {
                        model.flag = false;
                        model.message = "Empty table, Repo is sick.";
                        return Ok(model);
                    }
                    //We are sending the cart Id to front to get the number of products on cart.
                    //pOJOCartLine.id = model.id; (we dont need it anymore we are using UserId.)
                    //We are returing the updated wty from product
                    return Ok(pOJOCartLine);
                }
                else //Use the old cart to add product.
                {
                    //Proccess to cart line (create cart line )
                    cartLine.cartId = Convert.ToInt32(openedCart.id);
                    cartLine.price = product.price;
                    cartLine.freeTax = product.freeTax;
                    cartLine.freeShipping = product.freeShipping;
                    POJO pOJOCartLine = await unitOfWork.CartLine.Save(cartLine);

                    if (pOJOCartLine == null)
                    {
                        pOJOCartLine.flag = false;
                        pOJOCartLine.message = "Empty table, Repo is sick.";
                        return Ok(model);
                    }

                    openedCart.modifiedBy = cartLine.modifiedBy;
                    POJO cartModel = await unitOfWork.Cart.Save(openedCart);
                    if (cartModel == null)
                    {
                        cartModel.flag = false;
                        cartModel.message = "Empty table, Repo is sick.";
                        return Ok(cartModel);
                    }
                    //retrun the cart Id.
                    //  pOJOCartLine.id = model.id; we are using the userId rather than using CartId
                    //We are returing the updated wty from product
                    return Ok(pOJOCartLine);
                }
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = $"APIs function is sick, Error {ex.ToString()}";
                return Ok(model);
            }
        }

        //Get the number of items in MyCart by cart Id
        [HttpGet("NumberOfProducts/{id}")]
        public async Task<IActionResult> NumberOfProducts(string id)
        {
            CartLine cartLineNumbers = new CartLine();

            if (id == null)
            {
                cartLineNumbers.id = 0;
                return Ok(cartLineNumbers);
            }

            try
            {
                //Check if the user has open cart.
                var openedCarts = await unitOfWork.Cart.GetAll(x => x.createdBy.Equals(id) && x.isDeleted.Equals(false));
                var openedCart = openedCarts?.FirstOrDefault(x => x.status.Equals(Status.Open));
                if (openedCart == null)
                {
                    cartLineNumbers.id = 0;
                    return Ok(cartLineNumbers);
                }
                var model = await unitOfWork.CartLine.GetAll(x => x.cartId.Equals(openedCart.id));
                //We are useing Id just to pass the number of products.
                //rather than create new model
                cartLineNumbers.id = model.Count();
                return Ok(cartLineNumbers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        //This method will get the current opend cart for the user
        [HttpGet("GetMyCart/{id}")]
        public async Task<IActionResult> GetMyCart(string id)
        {
            //Get the opendCart
            CartLineDetails models = new CartLineDetails();
            //Check if the user has open cart.
            var openedCarts = await unitOfWork.Cart.GetAll(x => x.createdBy.Equals(id) && x.isDeleted.Equals(false));
            if (openedCarts != null)
            {
                models = await CartProduct(openedCarts);
            }
            return Ok(models);
        }

        //CheckOut by UserId
        [HttpGet("CheckOut/{id}")]
        public async Task<IActionResult> CheckOut(string id)
        {
            CheckOut checkOut = new CheckOut();
            CartLineDetails models = new CartLineDetails();
            if (id == null)
            {
                return BadRequest();
            }
            //Get the opendCart           
            var openedCarts = await unitOfWork.Cart.GetAll(x => x.createdBy.Equals(id) && x.isDeleted.Equals(false));

            if (openedCarts is null)
            {
                return Ok(checkOut);
            }

            models = await CartProduct(openedCarts);

            if (models is null)
            {
                return Ok(checkOut);
            }

            User user = await serviceUser.Get(id, url, await serviceUser.GetAccessToken());

            var totalCart = TotalCart.GetTotalCart(models);

            if (user != null)
            {
                checkOut.email = user.email;
                checkOut.fullName = user.name;
                checkOut.phone = user.phoneNumber;
            }

            checkOut.userId = id;
            checkOut.total = totalCart.total;
            checkOut.status = models.cartStatus;
            checkOut.cartId = models.cartId;
            checkOut.totalFreeTaxProduct = totalCart.totalFreeTaxProduct;
            checkOut.totalTaxedProduct = totalCart.totalTaxedProduct;
            checkOut.subTotal = totalCart.subtotal;
            checkOut.shippingCost = totalCart.shippingCost;
            checkOut.tax = models.tax;
            checkOut.taxedCost = totalCart.taxCost;
            checkOut.isCard = models.isCard;
            checkOut.isCash = models.isCash;
            checkOut.couponCode = models.couponCode;
            checkOut.couponValue = models.couponValue;
            checkOut.discountType = models.discountType;
            checkOut.couponName = models.couponName;
            checkOut.couponEnabled = models.couponEnabled;
            checkOut.couponNumberOfUse = models.couponNumberOfUse;
            checkOut.couponStartOn = models.couponStartOn;
            checkOut.couponExpireOn = models.couponExpireOn;
            checkOut.couponMinimumSpend = models.couponMinimumSpend;
            checkOut.couponMaximumDiscount = models.couponMaximumDiscount;
            checkOut.couponHidden = totalCart.couponHidden;
            checkOut.discount = totalCart.cartDiscount;

            if (checkOut == null)
            {
                return NotFound();
            }
            return Ok(checkOut);
        }

        //Get Cart By CartId
        [HttpGet("GetCart/{id}")]
        public async Task<IActionResult> GetCart(string id)
        {
            //Get the opendCart
            //Check if the user has open cart.
            IQueryable<Cart> openedCarts = await unitOfWork.Cart.GetAll(x => x.id.Equals(Convert.ToInt32(id)));
            CartLineDetails models = await CartProduct(openedCarts);
            return Ok(models);
        }


        private async Task<CartLineDetails> CartProduct(IQueryable<Cart> openedCarts)
        {
            CartLineDetails models = new CartLineDetails();

            var cart = openedCarts?.FirstOrDefault(x => x.status.Equals(Status.Open) && x.isDeleted.Equals(false));
            if (cart != null)
            {
                var cartlines = await unitOfWork.CartLine.GetAll(w => w.cartId.Equals(cart.id));
                //Get Copoun By Id
                var coupon = await unitOfWork.Coupon.Get(cart.couponId);
                var medias = await unitOfWork.Media.GetAll(m => m.isDeleted.Equals(false));
                var products = await unitOfWork.Product.GetAll(p => p.isDeleted.Equals(false));
                var settings = await unitOfWork.Setting.GetAll();

                var cartLineProducts = from cartline in cartlines
                                       join product in products on cartline.productId equals product.id
                                       select new CartLineProduct
                                       {
                                           cartLineId = cartline.id,
                                           qty = cartline.qty,
                                           price = cartline.price,
                                           total = cartline.total,
                                           prodcutName = product.name,
                                           createdBy = cartline.createdBy,
                                           productImage = medias.FirstOrDefault(c => c.productId.Equals(product.id)).path,
                                           productId = product.id,
                                           freeShipping = product.freeShipping,
                                           freeTax = product.freeTax
                                       };

                if (cartLineProducts != null && cartLineProducts.Count() > 0)
                {
                    models.cartLineProduct.AddRange(cartLineProducts);
                }

                if (settings != null && settings.Count() > 0)
                {
                    models.tax = settings.FirstOrDefault().tax;
                    models.shippingCost = settings.FirstOrDefault().shippingCost;
                    models.couponEnabled = settings.FirstOrDefault().enableCoupon;
                    models.isCard = settings.FirstOrDefault().isCard;
                    models.isCash = settings.FirstOrDefault().isCash;
                }

                if (coupon != null)
                {
                    models.couponCode = coupon.code;
                    models.couponValue = coupon.value;
                    models.discountType = coupon.discountType;
                    models.couponName = coupon.couponName;
                    models.couponEnabled = coupon.isActive;
                    models.couponExpireOn = coupon.expireOn;
                    models.couponStartOn = coupon.startOn;
                    models.couponNumberOfUse = coupon.numberOfUse;
                    models.couponMaximumDiscount = coupon.maximumDiscount;
                    models.couponMinimumSpend = coupon.minimumSpend;
                }
                models.cartId = cart.id;
                models.cartStatus = cart.status;
            }
            return models;
        }

    }
}