using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ICouponHistory : IServices<CouponHistory>
    {
        Task<POJO> Save(CouponHistory couponHistory);
    }
}
