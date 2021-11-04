using ecommerce.Data;
using ecommerce.Data.Models;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class CouponNumberOfUseRepository : Repository<CouponNumberOfUse>, ICouponNumberOfUse
    {
        private readonly ApplicationDbContext _db;
        public CouponNumberOfUseRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(CouponNumberOfUse entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    entity.createDate = DateTime.Now;
                    await _db.CouponNumberOfUses.AddAsync(entity);
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
                CouponNumberOfUse _Entity = await _db.CouponNumberOfUses.FindAsync(entity.id);
              
                _Entity.couponId = entity.couponId.Equals(0) ? _Entity.couponId : entity.couponId;
                _Entity.numberOfUse = entity.numberOfUse.Equals(0) ? _Entity.numberOfUse : entity.numberOfUse;             
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
