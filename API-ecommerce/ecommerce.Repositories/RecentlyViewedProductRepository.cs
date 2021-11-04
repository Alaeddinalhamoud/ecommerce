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
    public class RecentlyViewedProductRepository : Repository<RecentlyViewedProduct>, IRecentlyViewedProduct
    {
        private readonly ApplicationDbContext _db;
        public RecentlyViewedProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(RecentlyViewedProduct entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    //Check if the product is already added
                    var isRecentlyViewedProduct = _db.RecentlyViewedProducts
                        .Where(x => x.userId.Equals(entity.userId) && x.productId.Equals(entity.productId)).Any();
                    if (!isRecentlyViewedProduct)
                    {
                        await _db.RecentlyViewedProducts.AddAsync(entity);
                        await _db.SaveChangesAsync();
                        model.id = entity.id.ToString();
                        model.flag = true;
                        model.message = "Has Been Added.";
                    }
                    else
                    {
                        model.flag = true;
                        model.message = "The prodcut already added.";
                    }
                   
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            else if (entity.id != 0)
            {
                RecentlyViewedProduct _Entity = await _db.RecentlyViewedProducts.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
                //_Entity.userId = entity.userId;
               // _Entity. = String.IsNullOrEmpty(entity.imagePath) ? _Entity.imagePath : entity.imagePath;
                //  _Entity.createDate = entity.createDate;
                _Entity.updateDate = DateTime.Now;            
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
