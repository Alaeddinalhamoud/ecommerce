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
    public class TrackingOrderRepository : Repository<TrackingOrder>, ITrackingOrder
    {
        private readonly ApplicationDbContext _db;
        public TrackingOrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(TrackingOrder entity)
        {
            POJO model = new POJO();
            //If the order already exist just update the order
            var OrderIdExist = _db.TrackingOrders.Where(x => x.orderId.Equals(entity.orderId)).Any();
            if (entity.id == 0 && !OrderIdExist)
            {
                try
                {
                    entity.date = DateTime.Now;//Need it here for first create
                    await _db.TrackingOrders.AddAsync(entity);
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
                TrackingOrder _Entity = await _db.TrackingOrders.FindAsync(entity.id);
                _Entity.orderId = String.IsNullOrEmpty(entity.orderId) ? _Entity.orderId : entity.orderId;
                _Entity.trackingStatus = entity.trackingStatus.Equals(0) ? _Entity.trackingStatus : entity.trackingStatus;
                _Entity.userId = String.IsNullOrEmpty(entity.userId) ? _Entity.userId : entity.userId;
                _Entity.updateDate = DateTime.Now;
                _Entity.courierTrackingNumber = String.IsNullOrEmpty(entity.courierTrackingNumber) ? _Entity.courierTrackingNumber : entity.courierTrackingNumber;
                _Entity.curierCopmany = String.IsNullOrEmpty(entity.curierCopmany) ? _Entity.curierCopmany : entity.curierCopmany;
                _Entity.trackingUrl = String.IsNullOrEmpty(entity.trackingUrl) ? _Entity.trackingUrl : entity.trackingUrl;
                _Entity.email = String.IsNullOrEmpty(entity.email) ? _Entity.email : entity.email;
                _Entity.expectedArrival = entity.expectedArrival.Equals("0001-01-01T00:00:00") ? _Entity.expectedArrival : entity.expectedArrival;
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
