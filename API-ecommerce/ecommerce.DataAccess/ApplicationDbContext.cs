using ecommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Wishlist> WishLists { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartLine> CartLines { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<ProductSpecification> ProductSpecification { get; set; }
        public DbSet<RecentlyViewedProduct> RecentlyViewedProducts { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; } 
        public DbSet<TrackingOrder>  TrackingOrders { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<MostViewedProduct> MostViewedProducts { get; set; }
        public DbSet<OrderAddress> OrderAddresses { get; set; }
        public DbSet<VendorApplication> VendorApplications { get; set; }
        public DbSet<VendorBank> VendorBanks { get; set; }
        public DbSet<VendorProfile> VendorProfiles { get; set; }
        public DbSet<VendorMedia> VendorMedias { get; set; }
        public DbSet<ShippingCountry>  ShippingCountries { get; set; }
        public DbSet<OrderReturnReason> OrderReturnReasons { get; set; }
        public DbSet<OrderReturnCase> OrderReturnCases { get; set; }
        public DbSet<OrderComplaint> OrderComplaints { get; set; }
        public DbSet<OrderComplaintCase> OrderComplaintCases { get; set; }
        public DbSet<MetaTag> MetaTags { get; set; }
        public DbSet<Page> Pages { get; set; } 
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponHistory> CouponHistories { get; set; }
        public DbSet<CouponNumberOfUse> CouponNumberOfUses { get; set; }
    }
}
