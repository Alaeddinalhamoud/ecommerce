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
   public class MostViewedProductRepository : Repository<MostViewedProduct>, IMostViewedProduct
    {

        private readonly ApplicationDbContext _db;
        public MostViewedProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(MostViewedProduct entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    //Check if the product is already added
                    var OldMostViewedProduct = _db.MostViewedProducts
                        .FirstOrDefault(x => x.productId.Equals(entity.productId));
                    if (OldMostViewedProduct == null)
                    {
                        await _db.MostViewedProducts.AddAsync(entity);
                        await _db.SaveChangesAsync();
                        model.id = entity.id.ToString();
                        model.flag = true;
                        model.message = "Has Been Added.";
                    }
                    else
                    {
                        //Need to update the Freq.
                        model = await UpdateMostViewedProduct(OldMostViewedProduct);
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
             model =  await UpdateMostViewedProduct(entity);
            }
            return model;
        }

        private async Task<POJO> UpdateMostViewedProduct(MostViewedProduct entity)
        {
            POJO model = new POJO();
            MostViewedProduct _Entity = await _db.MostViewedProducts.FindAsync(entity.id);
            _Entity.id = entity.id;
            _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
            _Entity.frequency = _Entity.frequency + 1;
            _Entity.lastVisitDate = DateTime.Now;
            try
            {
                await _db.SaveChangesAsync();
                model.id = _Entity.id.ToString();
                model.flag = true;
                model.message = "Has Been Updated.";
                return model;
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = ex.ToString();
                return model;
            } 
        }
    }
}
