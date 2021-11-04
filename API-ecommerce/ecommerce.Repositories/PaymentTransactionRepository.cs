using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransaction
    {
        private readonly ApplicationDbContext _db;
        public PaymentTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(PaymentTransaction entity)
        {
            POJO model = new POJO();

            try
            {
                entity.createDate = DateTime.Now;//Need it here for first create
                await _db.PaymentTransactions.AddAsync(entity);
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

            //else if (!String.IsNullOrEmpty(entity.id))
            //{
            //    PaymentTransaction _Entity = await _db.PaymentTransactions.FindAsync(entity.id);
            //    _Entity.amount = entity.amount.Equals(0) ? _Entity.amount : entity.amount;
            //    _Entity.fullName = String.IsNullOrEmpty(entity.fullName) ? _Entity.fullName : entity.fullName;
            //    _Entity.orderId = String.IsNullOrEmpty(entity.orderId) ? _Entity.orderId : entity.orderId;
            //    _Entity.status = String.IsNullOrEmpty(entity.status) ? _Entity.status : entity.status;
            //    _Entity.userId = String.IsNullOrEmpty(entity.userId) ? _Entity.userId : entity.userId;

            //    try
            //    {
            //        await _db.SaveChangesAsync();
            //        model.id = _Entity.id.ToString();
            //        model.flag = true;
            //        model.message = "Has Been Updated.";
            //    }
            //    catch (Exception ex)
            //    {
            //        model.flag = false;
            //        model.message = ex.ToString();
            //    }
            //}
            return model;
        }
    }
}
