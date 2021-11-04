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
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IService<User> serviceUser;
        private readonly string url = "/api/User/";
        public OrderController(IUnitOfWork _UnitOfWork, IService<User> _serviceUser)
        {
            unitOfWork = _UnitOfWork;
            serviceUser = _serviceUser;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            try
            {
                POJO model = new POJO();
                if (order == null)
                {
                    model.flag = false;
                    model.message = "Empty request.";
                    return Ok(model);
                }

                //get the total
                var cart = await unitOfWork.Cart.Get(order.cartId);

                if (cart.status.Equals(Status.Closed))
                {
                    model.flag = false;
                    model.message = "Cart already closed.";
                    //Find the cart in order tb.
                    model.id = unitOfWork.Order.GetAll(x => x.cartId.Equals(order.cartId)).Result.FirstOrDefault().id;
                    return Ok(model);
                }
                var models = await CartProduct(cart);

                if (models != null)
                {
                    var totalCart = TotalCart.GetTotalCart(models);

                    order.subTotal = totalCart.subtotal;
                    order.shippingCost = totalCart.shippingCost;
                    order.tax = totalCart.tax;
                    //Tax is number on the dash settings
                    order.taxCost = totalCart.taxCost;
                    order.total = totalCart.total;
                    order.discount = totalCart.cartDiscount;
                    order.status = Status.Open;
                }

                model = await unitOfWork.Order.Save(order);

                if (model == null)
                {
                    model.flag = false;
                    model.message = "APIs function is sick.";
                    return Ok(model);
                }

                var cartlines = await unitOfWork.CartLine.GetAll(x => x.cartId.Equals(order.cartId));

                List<OrderLine> orderLineslst = new List<OrderLine>();

                foreach (var item in cartlines)
                {
                    OrderLine orderLine = new OrderLine()
                    {
                        orderId = model.id,
                        productId = item.productId,
                        createDate = DateTime.Now,
                        price = item.price,
                        qty = item.qty,
                        modifiedBy = order.createdBy,
                        freeShipping = item.freeShipping,
                        freeTax = item.freeTax
                    };
                    orderLineslst.Add(orderLine);
                }

                await unitOfWork.OrderLine.SaveRange(orderLineslst);

                //close cart.
                cart.status = Status.Closed;
                cart.modifiedBy = order.createdBy;
                await unitOfWork.Cart.Save(cart);

                //Update and added Coupon History
                await unitOfWork.CouponHistory.Save(new CouponHistory { couponId = cart.couponId, orderId = model.id, createDate = DateTime.Now, createdBy = cart.createdBy });
                var couponNumberOfUses = await unitOfWork.CouponNumberOfUse.GetAll(x => x.couponId.Equals(cart.couponId));
                if(couponNumberOfUses != null && couponNumberOfUses.Count() > 0)
                {
                    var couponNumberOfUse = couponNumberOfUses?.FirstOrDefault();
                    couponNumberOfUse.numberOfUse = +1;
                    couponNumberOfUse.modifiedBy = cart.createdBy;
                    await unitOfWork.CouponNumberOfUse.Save(couponNumberOfUse);
                }
                else
                {
                    await unitOfWork.CouponNumberOfUse.Save(new CouponNumberOfUse {couponId = cart.couponId, createdBy = cart.createdBy, numberOfUse = 1, createDate = DateTime.Now });
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Issue will trying to create order, Cart Number {order.cartId}, Error message : {ex}");
            }
        }

        [HttpPost("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] Order order)
        {
            POJO model = new POJO();

            if (order == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Order.Save(order);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }
            return Ok(model);
        }

        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder([FromBody] Order order)
        {
            POJO model = new POJO();

            if (order == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
            //Update Order to mark as deleted
            model = await unitOfWork.Order.Save(order);

            //Updated the tracking           
            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            var orderTracking = await unitOfWork.TrackingOrder.GetAll(x => x.orderId.Equals(order.id));

            if(orderTracking != null && orderTracking.Count() > 0)
            {
                var tracking = orderTracking?.FirstOrDefault();
                tracking.trackingStatus = TrackingStatus.Deleted;

               _ = await unitOfWork.TrackingOrder.Save(tracking);
            }

            return Ok(model);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = await unitOfWork.Order.GetAll(x => x.id.Equals(id));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model?.FirstOrDefault());
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

            model = await unitOfWork.Order.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }
        [HttpGet("GetDeletedOrder/{id}")]
        public async Task<IActionResult> GetDeletedOrder(string id)
        {
            OrderDetails models = new OrderDetails();
            var orders = await unitOfWork.Order.GetAll(ord => ord.id.Equals(id));

            if (orders is null && orders.Count() < 0)
                return NotFound("Empty Order");

            var orderLines = await unitOfWork.OrderLine.GetAll(x => x.orderId.Equals(id) && x.isDeleted.Equals(false));

            if (orderLines == null)
                return NotFound("No orderlines");

            await OrderDetails(id, models, orders, orderLines);

            return Ok(models);
        }

            [HttpGet("YourOrder/{id}")]
        public async Task<IActionResult> YourOrder(string id)
        {
            OrderDetails models = new OrderDetails();
            var orders = await unitOfWork.Order.GetAll(ord => ord.id.Equals(id) && ord.isDeleted.Equals(false));

            if (orders is null && orders.Count() < 0)
                return NotFound("Empty Order");

            var orderLines = await unitOfWork.OrderLine.GetAll(x => x.orderId.Equals(id) && x.isDeleted.Equals(false));

            if (orderLines == null)
                return NotFound("No orderlines");

            await OrderDetails(id, models, orders, orderLines);

            return Ok(models);
        }

        private async Task OrderDetails(string id, OrderDetails models, IQueryable<Order> orders, IQueryable<OrderLine> orderLines)
        {
            var orderTrackingStatus = await unitOfWork.TrackingOrder.GetAll(x => x.orderId.Equals(id));

            var paymentTransaction = await unitOfWork.PaymentTransaction.GetAll(pa => pa.orderId.Equals(id));

            var medias = await unitOfWork.Media.GetAll(m => m.isDeleted.Equals(false));
            var products = await unitOfWork.Product.GetAll(p => p.isDeleted.Equals(false));
            var user = await serviceUser.Get(orders?.FirstOrDefault()?.createdBy, url, await serviceUser.GetAccessToken());
            var settings = await unitOfWork.Setting.GetAll();
            //We store the Address in OrderAddresses. get it by order Id
            var orderAddress = await unitOfWork.OrderAddress.GetAll(x => x.orderId.Equals(id));
            var orderLineProduct = from orderLine in orderLines
                                   join product in products on orderLine.productId equals product.id
                                   select new OrderLineProduct
                                   {
                                       orderLineId = orderLine.id,
                                       prodcutName = product.name,
                                       createdBy = orderLine.modifiedBy,
                                       modifiedBy = orderLine.modifiedBy,
                                       price = orderLine.price,
                                       productImage = medias.FirstOrDefault(c => c.productId.Equals(product.id)).path,
                                       qty = orderLine.qty,
                                       productId = product.id,
                                       freeTax = orderLine.freeTax,
                                       freeShipping = orderLine.freeShipping,
                                   };

            if (orderLineProduct != null && orderLineProduct.Count() > 0)
            {
                models.orderLineProducts.AddRange(orderLineProduct);
            }

            if (orders != null && orders.Count() > 0)
            {
                models.orderId = orders?.FirstOrDefault().id;
                models.isPaid = orders.FirstOrDefault().isPaid;
                models.trackingStatus = orders.FirstOrDefault().trackingStatus;
                models.paymentMethod = orders.FirstOrDefault().paymentMethod;
                models.subTotal = orders.FirstOrDefault().subTotal;
                models.shippingCost = orders.FirstOrDefault().shippingCost;
                models.tax = orders.FirstOrDefault().tax;
                models.taxCost = orders.FirstOrDefault().taxCost;
                models.total = orders.FirstOrDefault().total;
                models.discount = orders.FirstOrDefault().discount;
                models.createDate = orders.FirstOrDefault().createDate;
                models.Status = orders.FirstOrDefault().status;
                models.createdBy = orders.FirstOrDefault().createdBy;
            }

            if (settings != null)
            {
                models.orderReturnDays = settings.FirstOrDefault().orderReturnDays;
            }

            if (paymentTransaction != null)
            {
                models.paymentTransaction = String.IsNullOrEmpty(paymentTransaction?.FirstOrDefault()?.id) ? "UnPaid" : paymentTransaction?.FirstOrDefault().id;
            }

            if (orderAddress != null)
            {
                models.orderAddress = orderAddress.FirstOrDefault();
            }

            if (user != null)
            {
                models.FullName = user.name;
                models.email = user.email;
                models.phone = user.phoneNumber;
            }
            if (orderTrackingStatus != null && orderTrackingStatus.Count() > 0)
            {
                models.trackingOrder = orderTrackingStatus?.FirstOrDefault();
            }
        }

        [HttpGet("GetOrdersByStatus/{id}")]
        public async Task<IActionResult> GetOrdersByStatus(string id)
        {
            //Id is the status true or false
            IQueryable<Order> orders = await unitOfWork.Order.GetAll(x => x.isPaid.Equals(Convert.ToBoolean(id)) && x.isDeleted.Equals(false));
            return Ok(orders);
        }

        [HttpGet("GetDeletedOrders")]
        public async Task<IActionResult> GetDeletedOrders()
        {
            //Id is the status true or false
            var orders = await unitOfWork.Order.GetAll(x => x.isDeleted.Equals(true));
            return Ok(orders);
        }

        //OrderId
        [HttpGet("CheckOutOrder/{id}")]
        public async Task<IActionResult> CheckOutOrder(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Order order = unitOfWork.Order.GetAll(x => x.id.Equals(id))?.Result?.FirstOrDefault();
            // Address address = unitOfWork.Address.GetAll(x => x.createdBy.Equals(order.createdBy))?.Result?.FirstOrDefault();
            User user = await serviceUser.Get(order.createdBy, url, await serviceUser.GetAccessToken());
            var settings = await unitOfWork.Setting.GetAll();


            CheckOut checkOut = new CheckOut();

            checkOut.email = user.email;
            checkOut.fullName = user.name;
            checkOut.phone = user.phoneNumber;
            checkOut.userId = order.createdBy;
            checkOut.total = order.total;
            checkOut.orderId = order.id;
            checkOut.subTotal = order.subTotal;
            checkOut.shippingCost = order.shippingCost;
            checkOut.tax = order.tax;
            checkOut.taxedCost = order.taxCost;
            checkOut.isPaid = order.isPaid;
            checkOut.isCard = settings.FirstOrDefault().isCard;
            checkOut.isCash = settings.FirstOrDefault().isCash;
            checkOut.discount = order.discount;

            if (checkOut == null)
            {
                return NotFound();
            }
            return Ok(checkOut);
        }

        private async Task<CartLineDetails> CartProduct(Cart cart)
        {
            CartLineDetails models = new CartLineDetails();
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