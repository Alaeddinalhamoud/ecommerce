using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class SettingRepository : Repository<Setting>, ISetting
    {
        private readonly ApplicationDbContext _db;
        public SettingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<POJO> Save(Setting entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    entity.createDate = DateTime.Now;//Need it here for first create
                    await _db.Settings.AddAsync(entity);
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
                Setting _Entity = await _db.Settings.FindAsync(entity.id);
                // _Entity.id = entity.id;
                _Entity.websiteName = String.IsNullOrEmpty(entity.websiteName) ? _Entity.websiteName : entity.websiteName;
                _Entity.numberOfSliders = entity.numberOfSliders.Equals(0) ? _Entity.numberOfSliders : entity.numberOfSliders;
                _Entity.pageSize = entity.pageSize.Equals(0) ? _Entity.pageSize : entity.pageSize;
                _Entity.email = String.IsNullOrEmpty(entity.email) ? _Entity.email : entity.email;
                _Entity.salesEmail = String.IsNullOrEmpty(entity.salesEmail) ? _Entity.salesEmail : entity.salesEmail;
                _Entity.helpDeskEmail = String.IsNullOrEmpty(entity.helpDeskEmail) ? _Entity.helpDeskEmail : entity.helpDeskEmail;
                _Entity.createdBy = String.IsNullOrEmpty(entity.createdBy) ? _Entity.createdBy : entity.createdBy;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ? _Entity.modifiedBy : entity.modifiedBy;
                _Entity.isSlider = entity.isSlider;
                _Entity.isMostViewdProduct = entity.isMostViewdProduct;
                _Entity.isNewArrivalProduct = entity.isNewArrivalProduct;
                _Entity.isRecentlyViewedProduct = entity.isRecentlyViewedProduct;
                _Entity.isTidioScript = entity.isTidioScript;
                _Entity.logo = String.IsNullOrEmpty(entity.logo) ? _Entity.logo : entity.logo;
                _Entity.description = String.IsNullOrEmpty(entity.description) ? _Entity.description : entity.description;
                _Entity.address = String.IsNullOrEmpty(entity.address) ? _Entity.address : entity.address;
                _Entity.phone = String.IsNullOrEmpty(entity.phone) ? _Entity.phone : entity.phone;
                _Entity.tidioScript = entity.tidioScript;
                _Entity.tax = entity.tax.Equals(0) ? _Entity.tax : entity.tax;
                _Entity.orderReturnDays = entity.orderReturnDays.Equals(0) ? _Entity.orderReturnDays : entity.orderReturnDays;
                _Entity.shippingCost = entity.shippingCost.Equals(0) ? _Entity.shippingCost : entity.shippingCost;
                _Entity.isCard = entity.isCard;
                _Entity.isCash = entity.isCash;
                _Entity.enableCoupon = entity.enableCoupon;
                _Entity.enableMaintenance = entity.enableMaintenance;
                _Entity.isVendorEnabled = entity.isVendorEnabled;
                _Entity.vendorAgreementContract = String.IsNullOrEmpty(entity.vendorAgreementContract) ? _Entity.vendorAgreementContract : entity.vendorAgreementContract;
                _Entity.payTabsMerchantProfileID = entity.payTabsMerchantProfileID.Equals(0) ? _Entity.payTabsMerchantProfileID : entity.payTabsMerchantProfileID;
                _Entity.payTabsServerKey = String.IsNullOrEmpty(entity.payTabsServerKey) ? _Entity.payTabsServerKey : entity.payTabsServerKey;
                _Entity.payTabsClientKey = String.IsNullOrEmpty(entity.payTabsClientKey) ? _Entity.payTabsClientKey : entity.payTabsClientKey;
                _Entity.payTabsAPIUrl = String.IsNullOrEmpty(entity.payTabsAPIUrl) ? _Entity.payTabsAPIUrl : entity.payTabsAPIUrl;
                _Entity.facebookLink = String.IsNullOrEmpty(entity.facebookLink) ? _Entity.facebookLink : entity.facebookLink;
                _Entity.twitterLink = String.IsNullOrEmpty(entity.twitterLink) ? _Entity.twitterLink : entity.twitterLink;
                _Entity.instgramLink = String.IsNullOrEmpty(entity.instgramLink) ? _Entity.instgramLink : entity.instgramLink;
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
