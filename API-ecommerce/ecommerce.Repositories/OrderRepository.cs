using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    class OrderRepository : Repository<Order>,IOrder
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Order entity)
        {

            POJO model = new POJO();           
            if (String.IsNullOrEmpty(entity.id))
            {
                try
                {
                    var orders = _db.Orders;
                    if(orders.Count().Equals(0))
                    {
                        entity.id = $"ORD-" + 1;
                    }
                    else
                    {
                        var lastOrder = orders?.OrderByDescending(t => t.id)?.FirstOrDefault();
                        entity.id = CreateOrderID(lastOrder.id);
                    } 

                    await _db.Orders.AddAsync(entity);
                    await _db.SaveChangesAsync();
                    model.id = entity.id;
                    model.flag = true;
                    model.message = "Has Been Added.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            //else if Student Id is not 0
            else if (!String.IsNullOrEmpty(entity.id))
            {
                 Order _Entity = await _db.Orders.FindAsync(entity.id);
                //_Entity.id = entity.id; 
                // _Entity.userId = entity.userId;
                // _Entity.vendorId = entity.vendorId;
                _Entity.description = String.IsNullOrEmpty(entity.description) ? _Entity.description : entity.description;
                _Entity.trackingStatus = entity.trackingStatus.Equals(0) ? _Entity.trackingStatus : entity.trackingStatus;
                _Entity.total = entity.total.Equals(0) ? _Entity.total : entity.total;
                _Entity.isPaid = entity.isPaid.Equals(false) ? _Entity.isPaid : entity.isPaid;
                _Entity.paymentMethod = entity.paymentMethod == 0 ? _Entity.paymentMethod : entity.paymentMethod;
                // _Entity.createDate = entity.createDate;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ? _Entity.modifiedBy : entity.modifiedBy;
                _Entity.cartId = entity.cartId.Equals(0) ? _Entity.cartId : entity.cartId;
                _Entity.tax = entity.tax.Equals(0) ? _Entity.tax : entity.tax;
                _Entity.taxCost = entity.taxCost.Equals(0) ? _Entity.taxCost : entity.taxCost;
                _Entity.shippingCost = entity.shippingCost.Equals(0) ? _Entity.shippingCost : entity.shippingCost;
                _Entity.subTotal = entity.subTotal.Equals(0) ? _Entity.subTotal : entity.subTotal;
                _Entity.isDeleted = entity.isDeleted.Equals(false) ? _Entity.isDeleted : entity.isDeleted;
                _Entity.status = entity.status == 0 ? _Entity.status : entity.status;
                _Entity.discount = entity.discount.Equals(0) ? _Entity.discount : entity.discount;
                try
                {
                    await _db.SaveChangesAsync();
                    model.id = _Entity.id;
                    model.flag = true;
                    model.message = "Has Been Updated.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            return model;
        }

        private string CreateOrderID(string id)
        {
            string _OrderString = "ORD-";
            string[] _OderSplit; 
            _OderSplit = id.Split('-');
            int i = Int32.Parse(_OderSplit[1]);
            i += 1;
            id = _OrderString + i;

            bool IsExist = _db.Orders.Any(e => e.id.Equals(id));
            if (IsExist)
            {
                id = CreateOrderID(id);
            }  
            return id;
        }


    }
}
