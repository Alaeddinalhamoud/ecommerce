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
  public  class ReviewRepository: Repository<Review>, IReview
    {
        private readonly ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Review entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    //check if already reviewd this product
                    var isReview = _db.Reviews
                        .Where(x => x.productId.Equals(entity.productId) && x.createdBy.Equals(entity.createdBy)).Any();
                    if (!isReview)
                    {
                        await _db.Reviews.AddAsync(entity);
                        await _db.SaveChangesAsync();
                        model.id = entity.id.ToString();
                        model.flag = true;
                        model.message = "Has Been Added.";
                    }
                    else
                    {
                        model.flag = false;
                        model.message = "Already Added.";
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
                Review _Entity = await _db.Reviews.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.rating = entity.rating.Equals(0) ? _Entity.rating : entity.rating;
               // _Entity.userId = entity.userId;
                _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
               // _Entity.userId = entity.userId;              
               // _Entity.createDate = entity.createDate;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ? _Entity.modifiedBy : entity.modifiedBy;
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
                _Entity.description = String.IsNullOrEmpty(entity.description) ? _Entity.description : entity.description;
                _Entity.email = String.IsNullOrEmpty(entity.email) ? _Entity.email : entity.email;
                _Entity.isApproved = entity.isApproved.Equals(false) ? _Entity.isApproved : entity.isApproved;
                _Entity.isDeleted = entity.isDeleted.Equals(false) ? _Entity.isDeleted : entity.isDeleted;
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
        public async Task<POJO> UpdateIsDelete(Review entity)
        {
            POJO model = new POJO();
            if (entity.id != 0)
            {
                Review _Entity = await _db.Reviews.FindAsync(entity.id);
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
