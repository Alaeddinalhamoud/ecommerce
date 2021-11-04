using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class CouponHistoryRepository : Repository<CouponHistory>, ICouponHistory
    {
        private readonly ApplicationDbContext _db;
        public CouponHistoryRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(CouponHistory entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    entity.createDate = DateTime.Now;
                    await _db.CouponHistories.AddAsync(entity);
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
                CouponHistory _Entity = await _db.CouponHistories.FindAsync(entity.id);
                _Entity.orderId = String.IsNullOrEmpty(entity.orderId) ? _Entity.orderId : entity.orderId;
                _Entity.couponId = entity.couponId.Equals(0) ? _Entity.couponId : entity.couponId;               
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