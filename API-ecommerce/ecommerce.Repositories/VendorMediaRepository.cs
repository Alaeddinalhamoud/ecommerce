using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class VendorMediaRepository : Repository<VendorMedia>, IVendorMedia
    {
        private readonly ApplicationDbContext _db;
        public VendorMediaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(VendorMedia entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.VendorMedias.AddAsync(entity);
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
                VendorMedia _Entity = await _db.VendorMedias.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
              //  _Entity.mediaType = entity.mediaType.Equals(0) ? _Entity.mediaType : entity.mediaType;
                _Entity.alt = String.IsNullOrEmpty(entity.alt) ? _Entity.alt : entity.alt;
                _Entity.path = String.IsNullOrEmpty(entity.path) ? _Entity.path : entity.path;
              //  _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
                //  _Entity.userId = entity.userId;               
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
    }
}
