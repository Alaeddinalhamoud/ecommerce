using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ICoupon : IServices<Coupon>
    {
        Task<POJO> Save(Coupon coupon);
    }
}
