using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;

namespace ecommerce.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            WishList = new WishlistRepository(_db);
            Order = new OrderRepository(_db);
            Product = new ProductRepository(_db);
            Review = new ReviewRepository(_db);
            OrderLine = new OrderLineRepository(_db);
            Brand = new BrandRepository(_db);
            Address = new AddressRepository(_db);
            ProductType = new ProductTypeRepository(_db);
            ProductSpecification = new ProductSpecificationRepository(_db);
            Media = new MediaRepository(_db);
            RecentlyViewedProduct = new RecentlyViewedProductRepository(_db);
            Cart = new CartRepository(_db);
            CartLine = new CartLineRepository(_db);
            Slider = new SliderRepository(_db);
            Setting = new SettingRepository(_db);
            PaymentTransaction = new PaymentTransactionRepository(_db);
            TrackingOrder = new TrackingOrderRepository(_db);
            FAQ = new FAQRepository(_db);
            Alert = new AlertRepository(_db);
            MostViewedProduct = new MostViewedProductRepository(_db);
            OrderAddress = new OrderAddressRepository(_db);
            VendorApplication = new VendorApplicationRepository(_db);
            VendorBank = new VendorBankRepository(_db);
            VendorProfile = new VendorProfileRepository(_db);
            VendorMedia = new VendorMediaRepository(_db);
            ShippingCountry = new ShippingCountryRepository(_db);
            OrderReturnReason = new OrderReturnReasonRepository(_db);
            OrderReturnCase = new OrderReturnCaseRepository(_db);
            OrderComplaint = new OrderComplaintRepository(_db);
            OrderComplaintCase = new OrderComplaintCaseRepository(_db);
            MetaTag = new MetaTagRepository(_db);
            Page = new PageRepository(_db);
            Coupon = new CouponRepository(_db);
            CouponNumberOfUse = new CouponNumberOfUseRepository(_db);
            CouponHistory = new CouponHistoryRepository(_db);
        }

        public ICategory Category { get; private set; }

        public IWishist WishList { get; private set; }

        public IOrder Order { get; private set; }

        public IProduct Product { get; private set; }

        public IReview Review { get; private set; }
        public IAddress Address { get; private set; }
        public IBrand Brand { get; private set; }
        public IMedia Media { get; private set; }
        public ICart Cart { get; private set; }
        public ICartLine CartLine { get; private set; }
        public IOrderLine OrderLine { get; private set; }
        public IProductType ProductType { get; private set; }
        public IProductSpecification ProductSpecification { get; private set; }
        public IRecentlyViewedProduct RecentlyViewedProduct { get; private set; }
        public ISetting Setting { get; private set; }
        public ISlider Slider { get; private set; }
        public IPaymentTransaction PaymentTransaction { get; private set; }
        public ITrackingOrder TrackingOrder { get; private set; }
        public IFAQ FAQ { get; private set; }
        public IAlert Alert { get; private set; }
        public IMostViewedProduct MostViewedProduct { get; private set; }
        public IOrderAddress OrderAddress { get; private set; }
        public IVendorApplication VendorApplication { get; private set; }
        public IVendorBank VendorBank { get; private set; }
        public IVendorProfile VendorProfile { get; private set; }
        public IVendorMedia VendorMedia { get; private set; }
        public IShippingCountry ShippingCountry { get; private set; }
        public IOrderReturnReason OrderReturnReason { get; private set; }
        public IOrderReturnCase OrderReturnCase { get; private set; }
        public IOrderComplaint OrderComplaint { get; private set; }
        public IOrderComplaintCase OrderComplaintCase { get; private set; }
        public IMetaTag MetaTag { get; private set; }
        public IPage Page { get; private set; }
        public ICoupon Coupon { get; private set; }
        public ICouponHistory CouponHistory { get; private set; }
        public ICouponNumberOfUse CouponNumberOfUse { get; private set; }


        public void Dispose() => _db.Dispose();
    }
}