using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class ProductTypeRepository : Repository<ProductType>, IProductType
    {
        private readonly ApplicationDbContext _db;
        public ProductTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(ProductType entity)
        {
            POJO model = new POJO();
            try
            {
                var isProductType = _db.ProductType.Where(x => x.name.Equals(entity.name)).Any();
                if (isProductType)
                {
                    model.flag = false;
                    model.message = $"{entity.name} ProductType Already Exist.";
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = ex.ToString();
            }



            if (entity.id == 0)
            {
                try
                {
                    await _db.ProductType.AddAsync(entity);
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
                ProductType _Entity = await _db.ProductType.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
                // _Entity.userId = entity.userId;
                //_Entity.createDate = entity.createDate;
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
        public async Task<POJO> UpdateIsDelete(ProductType entity)
        {
            POJO model = new POJO();
            if (entity.id != 0)
            {
                ProductType _Entity = await _db.ProductType.FindAsync(entity.id);
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
