using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class OrderReturnCaseRepository : Repository<OrderReturnCase>, IOrderReturnCase
    {

        private readonly ApplicationDbContext _db;
        public OrderReturnCaseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(OrderReturnCase entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.OrderReturnCases.AddAsync(entity);
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
                 OrderReturnCase _Entity = await _db.OrderReturnCases.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.orderId = String.IsNullOrEmpty(entity.orderId) ? _Entity.orderId : entity.orderId;
                _Entity.customerNote = String.IsNullOrEmpty(entity.customerNote) ? _Entity.customerNote : entity.customerNote;
                _Entity.messageToCustomer = String.IsNullOrEmpty(entity.messageToCustomer) ? _Entity.messageToCustomer : entity.messageToCustomer;
                _Entity.reasonId = entity.reasonId.Equals(0) ? _Entity.reasonId : entity.reasonId;
                _Entity.status = entity.status == 0 ? _Entity.status : entity.status;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ? _Entity.modifiedBy : entity.modifiedBy;
                _Entity.imageUrl = String.IsNullOrEmpty(entity.imageUrl) ? _Entity.imageUrl : entity.imageUrl;
                _Entity.email = String.IsNullOrEmpty(entity.email) ? _Entity.email : entity.email;
                _Entity.actionsToSolve = String.IsNullOrEmpty(entity.actionsToSolve) ? _Entity.actionsToSolve : entity.actionsToSolve;
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
