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
    public class CouponRepository : Repository<Coupon>, ICoupon
    {
        private readonly ApplicationDbContext _db;
        public CouponRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(Coupon entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    entity.createDate = DateTime.Now;
                    await _db.Coupons.AddAsync(entity);
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
                Coupon _Entity = await _db.Coupons.FindAsync(entity.id);
                // _Entity.id = entity.id;
                _Entity.couponName = String.IsNullOrEmpty(entity.couponName) ? _Entity.couponName : entity.couponName;
                _Entity.code = String.IsNullOrEmpty(entity.code) ? _Entity.code : entity.code;
                _Entity.discountType = entity.discountType == 0 ? _Entity.discountType : entity.discountType;
                _Entity.numberOfUse = entity.numberOfUse.Equals(0) ? _Entity.numberOfUse : entity.numberOfUse;
                _Entity.value = entity.value.Equals(0) ? _Entity.value : entity.value;
                _Entity.startOn = entity.startOn.Equals(StaticValues.NullDateTime) ? _Entity.startOn : entity.startOn;
                _Entity.expireOn = entity.expireOn.Equals(StaticValues.NullDateTime) ? _Entity.expireOn : entity.expireOn;
                _Entity.minimumSpend = entity.minimumSpend.Equals(0) ? _Entity.minimumSpend : entity.minimumSpend;
                _Entity.maximumDiscount = entity.maximumDiscount.Equals(0) ? _Entity.maximumDiscount : entity.maximumDiscount;
                _Entity.isActive = entity.isActive;
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
