using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class SliderRepository : Repository<Slider>, ISlider
    {
        private readonly ApplicationDbContext _db;
        public SliderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Slider entity)
        {
            POJO model = new POJO();
            //Add if  StudentId=0
            if (entity.id == 0)
            {
                try
                { 
                        await _db.Sliders.AddAsync(entity);
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
            //else if Student Id is not 0
            else if (entity.id != 0)
            {
                Slider _Entity = await _db.Sliders.FindAsync(entity.id);
                _Entity.id = entity.id; 
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
                _Entity.description = String.IsNullOrEmpty(entity.description) ? _Entity.description : entity.description;
                _Entity.imagePath = String.IsNullOrEmpty(entity.imagePath) ? _Entity.imagePath : entity.imagePath;
                _Entity.link = String.IsNullOrEmpty(entity.link) ? _Entity.link : entity.link;
                _Entity.buttonCaption = String.IsNullOrEmpty(entity.buttonCaption) ? _Entity.buttonCaption : entity.buttonCaption;
                _Entity.isEnabled = entity.isEnabled;
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
