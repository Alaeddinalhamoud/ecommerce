using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    class CartRepository : Repository<Cart>, ICart
    {
        private readonly ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Cart entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.Carts.AddAsync(entity);
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
                Cart _Entity = await _db.Carts.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.status = entity.status == 0 ? _Entity.status : entity.status;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ? _Entity.modifiedBy : entity.modifiedBy;
                _Entity.isDeleted = entity.isDeleted.Equals(false) ? _Entity.isDeleted : entity.isDeleted;
                _Entity.couponId = entity.couponId;
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
        public async Task<POJO> UpdateIsDelete(Cart entity)
        {
            POJO model = new POJO();
            if (entity.id != 0)
            {
                Cart _Entity = await _db.Carts.FindAsync(entity.id);
                _Entity.isDeleted = entity.isDeleted;
                _Entity.updateDate = entity.updateDate;
                _Entity.modifiedBy = entity.modifiedBy;
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
