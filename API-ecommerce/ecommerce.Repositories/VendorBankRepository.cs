using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class VendorBankRepository : Repository<VendorBank>, IVendorBank
    {
        private readonly ApplicationDbContext _db;
        public VendorBankRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(VendorBank entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.VendorBanks.AddAsync(entity);
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
                VendorBank _Entity = await _db.VendorBanks.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.vendorProfileId = entity.vendorProfileId.Equals(0) ? _Entity.vendorProfileId : entity.vendorProfileId;
                _Entity.bankName = String.IsNullOrEmpty(entity.bankName) ? _Entity.bankName : entity.bankName;
                _Entity.bankAddress = String.IsNullOrEmpty(entity.bankAddress) ? _Entity.bankAddress : entity.bankAddress;
                _Entity.account = String.IsNullOrEmpty(entity.account) ? _Entity.account : entity.account;
                _Entity.swiftCode = String.IsNullOrEmpty(entity.swiftCode) ? _Entity.swiftCode : entity.swiftCode;
                _Entity.iBAN = String.IsNullOrEmpty(entity.iBAN) ? _Entity.iBAN : entity.iBAN;
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
