using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class PageRepository : Repository<Page>, IPage
    {

        private readonly ApplicationDbContext _db;
        public PageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Page entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.Pages.AddAsync(entity);
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
                Page _Entity = await _db.Pages.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
                _Entity.content = String.IsNullOrEmpty(entity.content) ? _Entity.content : entity.content;
                _Entity.nav = entity.nav == 0 ? _Entity.nav : entity.nav;
                _Entity.published = entity.published; 
                _Entity.isPrivate = entity.isPrivate;
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
