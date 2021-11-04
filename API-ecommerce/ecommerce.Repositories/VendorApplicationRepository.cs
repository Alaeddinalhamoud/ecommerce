using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class VendorApplicationRepository : Repository<VendorApplication>, IVendorApplication
    {
        private readonly ApplicationDbContext _db;
        public VendorApplicationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(VendorApplication entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.VendorApplications.AddAsync(entity);
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
                VendorApplication _Entity = await _db.VendorApplications.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.fullName = String.IsNullOrEmpty(entity.fullName) ? _Entity.fullName : entity.fullName;
                _Entity.companyName = String.IsNullOrEmpty(entity.companyName) ? _Entity.companyName : entity.companyName;
                _Entity.companyVAT = String.IsNullOrEmpty(entity.companyVAT) ? _Entity.companyVAT : entity.companyVAT; 
                _Entity.workEmail = String.IsNullOrEmpty(entity.workEmail) ? _Entity.workEmail : entity.workEmail;
                _Entity.tel1 = String.IsNullOrEmpty(entity.tel1) ? _Entity.tel1 : entity.tel1;
                _Entity.tel2 = String.IsNullOrEmpty(entity.tel2) ? _Entity.tel2 : entity.tel2;
                _Entity.crNumber = String.IsNullOrEmpty(entity.crNumber) ? _Entity.crNumber : entity.crNumber;
                _Entity.ownerIdNumber = String.IsNullOrEmpty(entity.ownerIdNumber) ? _Entity.ownerIdNumber : entity.ownerIdNumber;
                _Entity.note = String.IsNullOrEmpty(entity.note) ? _Entity.note : entity.note;
                _Entity.bankName = String.IsNullOrEmpty(entity.bankName) ? _Entity.bankName : entity.bankName;
                _Entity.bankAddress = String.IsNullOrEmpty(entity.bankAddress) ? _Entity.bankAddress : entity.bankAddress;
                _Entity.account = String.IsNullOrEmpty(entity.account) ? _Entity.account : entity.account;
                _Entity.swiftCode = String.IsNullOrEmpty(entity.swiftCode) ? _Entity.swiftCode : entity.swiftCode;
                _Entity.iBAN = String.IsNullOrEmpty(entity.iBAN) ? _Entity.iBAN : entity.iBAN;
                _Entity.companyAddress = String.IsNullOrEmpty(entity.companyAddress) ? _Entity.companyAddress : entity.companyAddress;
                _Entity.status = entity.status == 0 ? _Entity.status : entity.status;
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
