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
    public class OrderAddressRepository : Repository<OrderAddress>, IOrderAddress
    {
        private readonly ApplicationDbContext _db;
        public OrderAddressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(OrderAddress entity)
        {
            POJO model = new POJO();
            //If the order already exist just update the order
            var OrderIdExist = _db.OrderAddresses.Where(x => x.orderId.Equals(entity.orderId)).Any();
            if (entity.id.Equals(0) && !OrderIdExist)
            {
                try
                {
                    entity.createDate = DateTime.Now;//Need it here for first create
                    await _db.OrderAddresses.AddAsync(entity);
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
            //We are using 
            else if (entity.id != 0 || !String.IsNullOrEmpty(entity.orderId))
            {
                OrderAddress _Entity = _db.OrderAddresses.FirstOrDefault(x => x.orderId.Equals(entity.orderId));
                _Entity.id = entity.id;
                _Entity.orderId = String.IsNullOrEmpty(entity.orderId) ? _Entity.orderId : entity.orderId;
                _Entity.houseNumber = String.IsNullOrEmpty(entity.houseNumber) ? _Entity.houseNumber : entity.houseNumber;
                _Entity.addressline1 = String.IsNullOrEmpty(entity.addressline1) ? _Entity.addressline1 : entity.addressline1;
                _Entity.addressline2 = String.IsNullOrEmpty(entity.addressline2) ? _Entity.addressline2 : entity.addressline2;
                _Entity.street = String.IsNullOrEmpty(entity.street) ? _Entity.street : entity.street;
                _Entity.city = String.IsNullOrEmpty(entity.city) ? _Entity.city : entity.city;
                _Entity.code = String.IsNullOrEmpty(entity.code) ? _Entity.code : entity.code;
                _Entity.country = String.IsNullOrEmpty(entity.country) ? _Entity.country : entity.country;
                _Entity.longitude = entity.longitude.Equals(0) ? _Entity.longitude : entity.longitude;
                _Entity.latitude = entity.latitude.Equals(0) ? _Entity.latitude : entity.latitude;

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
