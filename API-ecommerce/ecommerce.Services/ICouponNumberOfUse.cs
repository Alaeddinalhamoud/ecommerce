using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ICouponNumberOfUse : IServices<CouponNumberOfUse>
    {
        Task<POJO> Save(CouponNumberOfUse couponNumberOfUse);
    }
}
