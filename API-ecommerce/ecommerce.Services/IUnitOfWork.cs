using System;

namespace ecommerce.Services
{
    public interface IUnitOfWork : IDisposable
    {
        ICategory Category { get; }
        IWishist WishList { get; }
        IOrder Order { get; }
        IProduct Product { get; }
        IReview Review { get; }
        IMedia Media { get; }
        IBrand Brand { get; }
        IAddress Address { get; }
        ICart Cart { get; }
        ICartLine CartLine {get;}
        IOrderLine OrderLine { get;}
        IProductType ProductType { get;}
        IProductSpecification ProductSpecification { get;}
        IRecentlyViewedProduct RecentlyViewedProduct { get; }
        ISlider Slider { get; }
        ISetting Setting { get; }
        IPaymentTransaction PaymentTransaction { get; }
        ITrackingOrder TrackingOrder { get; }
        IAlert Alert { get; }
        IFAQ FAQ { get; }
        IMostViewedProduct MostViewedProduct { get; }
        IOrderAddress OrderAddress { get;}
        IVendorApplication VendorApplication { get; }
        IVendorBank VendorBank { get; }
        IVendorProfile VendorProfile { get; }
        IVendorMedia VendorMedia { get; }
        IShippingCountry ShippingCountry { get; }
        IOrderReturnReason OrderReturnReason { get; }
        IOrderReturnCase OrderReturnCase { get; }
        IOrderComplaint OrderComplaint { get; }
        IOrderComplaintCase OrderComplaintCase { get; }
        IMetaTag MetaTag { get; }
        IPage Page { get; }
        ICoupon Coupon { get; }
        ICouponHistory  CouponHistory { get; }
        ICouponNumberOfUse CouponNumberOfUse { get; }
    }
}
