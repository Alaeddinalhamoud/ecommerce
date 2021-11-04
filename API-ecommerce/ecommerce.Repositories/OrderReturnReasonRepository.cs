using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class OrderReturnReasonRepository : Repository<OrderReturnReason>, IOrderReturnReason
    {

        private readonly ApplicationDbContext _db;
        public OrderReturnReasonRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(OrderReturnReason entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.OrderReturnReasons.AddAsync(entity);
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
            else if (entity.id != 0)
            {
                OrderReturnReason _Entity = await _db.OrderReturnReasons.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.reason = String.IsNullOrEmpty(entity.reason) ? _Entity.reason : entity.reason;
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
    }
}
