using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class OrderLineRepository : Repository<OrderLine>, IOrderLine
    {
        private readonly ApplicationDbContext _db;
        public OrderLineRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(OrderLine entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.OrderLines.AddAsync(entity);
                    await _db.SaveChangesAsync();
                    model.id = entity.id.ToString();
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
            else if (entity.id !=0 )
            {
                OrderLine _Entity = await _db.OrderLines.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
                _Entity.orderId = entity.orderId.Equals(0) ? _Entity.orderId : entity.orderId;
                _Entity.qty = entity.qty.Equals(0) ? _Entity.qty : entity.qty;
                _Entity.price = entity.price.Equals(0) ? _Entity.price : entity.price;
                _Entity.freeShipping = entity.freeShipping;
                _Entity.freeTax = entity.freeTax;
                //  _Entity.createDate = entity.createDate;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ? _Entity.modifiedBy : entity.modifiedBy;
                try
                {
                    await _db.SaveChangesAsync();
                    model.id = _Entity.id.ToString();
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

        public async Task<POJO> SaveRange(List<OrderLine> entity)
        {
            POJO model = new POJO();
            try
            {
                await _db.OrderLines.AddRangeAsync(entity);
                await _db.SaveChangesAsync();

                model.flag = true;
                model.message = "Has Been Added.";
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = ex.ToString();
            }
            return model;
        }
    }
}
