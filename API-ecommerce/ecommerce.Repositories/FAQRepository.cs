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
    public class FAQRepository : Repository<FAQ>, IFAQ
    {
        private readonly ApplicationDbContext _db;
        public FAQRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(FAQ entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.FAQs.AddAsync(entity);
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
                FAQ _Entity = await _db.FAQs.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.question = String.IsNullOrEmpty(entity.question) ? _Entity.question : entity.question; 
                _Entity.answer = String.IsNullOrEmpty(entity.answer) ? _Entity.answer : entity.answer; 
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