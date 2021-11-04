using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class VendorProfileRepository : Repository<VendorProfile>, IVendorProfile
    {
        private readonly ApplicationDbContext _db;
        public VendorProfileRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(VendorProfile entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    await _db.VendorProfiles.AddAsync(entity);
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
                VendorProfile _Entity = await _db.VendorProfiles.FindAsync(entity.id);
                _Entity.id = entity.id; 
                _Entity.vendorId = String.IsNullOrEmpty(entity.vendorId) ? _Entity.vendorId : entity.vendorId;
                _Entity.companyName = String.IsNullOrEmpty(entity.companyName) ? _Entity.companyName : entity.companyName;
                _Entity.companyVAT = String.IsNullOrEmpty(entity.companyVAT) ? _Entity.companyVAT : entity.companyVAT; 
                _Entity.workEmail = String.IsNullOrEmpty(entity.workEmail) ? _Entity.workEmail : entity.workEmail;
                _Entity.tel1 = String.IsNullOrEmpty(entity.tel1) ? _Entity.tel1 : entity.tel1;
                _Entity.tel2 = String.IsNullOrEmpty(entity.tel2) ? _Entity.tel2 : entity.tel2;
                _Entity.crNumber = String.IsNullOrEmpty(entity.crNumber) ? _Entity.crNumber : entity.crNumber;
                _Entity.ownerIdNumber = String.IsNullOrEmpty(entity.ownerIdNumber) ? _Entity.ownerIdNumber : entity.ownerIdNumber; 
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
