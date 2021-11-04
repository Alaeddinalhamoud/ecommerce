using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class MetaTagRepository : Repository<MetaTag>, IMetaTag
    {
        private readonly ApplicationDbContext _db;
        public MetaTagRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(MetaTag entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.MetaTags.AddAsync(entity);
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
                MetaTag _Entity = await _db.MetaTags.FindAsync(entity.id);
              //  _Entity.id = entity.id;
                _Entity.title = String.IsNullOrEmpty(entity.title) ? _Entity.title : entity.title;
                _Entity.description = String.IsNullOrEmpty(entity.description) ? _Entity.description : entity.description;
                _Entity.type = String.IsNullOrEmpty(entity.type) ? _Entity.type : entity.type;
                _Entity.image = String.IsNullOrEmpty(entity.image) ? _Entity.image : entity.image;
                _Entity.imageAlt = String.IsNullOrEmpty(entity.imageAlt) ? _Entity.imageAlt : entity.imageAlt;
                _Entity.url = String.IsNullOrEmpty(entity.url) ? _Entity.url : entity.url;
                _Entity.locale = String.IsNullOrEmpty(entity.locale) ? _Entity.locale : entity.locale;
                _Entity.sitename = String.IsNullOrEmpty(entity.sitename) ? _Entity.sitename : entity.sitename;
                _Entity.video = String.IsNullOrEmpty(entity.video) ? _Entity.video : entity.video;
                _Entity.keywords = String.IsNullOrEmpty(entity.keywords) ? _Entity.keywords : entity.keywords;
                _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
                _Entity.metaTagType = entity.metaTagType == 0 ? _Entity.metaTagType : entity.metaTagType;
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
