using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class AlertRepository : Repository<Alert>, IAlert
    {

        private readonly ApplicationDbContext _db;
        public AlertRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(Alert entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.Alerts.AddAsync(entity);
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
                Alert _Entity = await _db.Alerts.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.title = String.IsNullOrEmpty(entity.title) ? _Entity.title : entity.title;
                _Entity.body = String.IsNullOrEmpty(entity.body) ? _Entity.body : entity.body;
                _Entity.alertType = entity.alertType.Equals(0) ? _Entity.alertType : entity.alertType;
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
