using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class ProductSpecificationRepository : Repository<ProductSpecification>, IProductSpecification
    {
        private readonly ApplicationDbContext _db;

        public ProductSpecificationRepository(ApplicationDbContext db):base(db)
        {
            _db=db;
        }
        public async Task<POJO> Save(ProductSpecification entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.ProductSpecification.AddAsync(entity);
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
                ProductSpecification _Entity = await _db.ProductSpecification.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
                _Entity.value = String.IsNullOrEmpty(entity.value) ? _Entity.value : entity.value;
                // _Entity.userId = entity.userId;
                // _Entity.createDate = entity.createDate;
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

        public async Task<POJO> SaveRange(List<ProductSpecification> entity)
        {
            POJO model = new POJO(); 
                try
                {
                    await _db.ProductSpecification.AddRangeAsync(entity);
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

        public async Task<POJO> UpdateIsDelete(ProductSpecification entity)
        {
            POJO model = new POJO();
            if (entity.id != 0)
            {
                ProductSpecification _Entity = await _db.ProductSpecification.FindAsync(entity.id);
                _Entity.isDeleted = entity.isDeleted;
                _Entity.updateDate = entity.updateDate;
                _Entity.modifiedBy = entity.modifiedBy;
                try
                {
                    await _db.SaveChangesAsync();
                    model.id = _Entity.productId.ToString();
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
