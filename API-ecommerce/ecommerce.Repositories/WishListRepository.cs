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
    class WishlistRepository : Repository<Wishlist>, IWishist
    {
        private readonly ApplicationDbContext _db;
        public WishlistRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Wishlist entity)
        {
            POJO model = new POJO();
            //Add if  StudentId=0
            if (entity.id == 0)
            {
                try
                {
                    //check if already is added
                    var isWishlist = _db.WishLists
                        .Where(x => x.productId.Equals(entity.productId) && x.userId.Equals(entity.userId)).Any();
                    if (!isWishlist)
                    {
                        await _db.WishLists.AddAsync(entity);
                        await _db.SaveChangesAsync();

                        model.id = entity.id.ToString();
                        model.flag = true;
                        model.message = "Has Been Added.";
                    }
                    else
                    {
                        model.flag = false;
                        model.message = "Already added.";
                    }
                   
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            //else if Student Id is not 0
            else if (entity.id != 0)
            {
                Wishlist _Entity = await _db.WishLists.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
                _Entity.userId = String.IsNullOrEmpty(entity.userId) ? _Entity.userId : entity.userId;
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
        
    }
}
