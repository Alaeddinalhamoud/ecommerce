using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IRecentlyViewedProduct : IServices<RecentlyViewedProduct>
    {
        Task<POJO> Save(RecentlyViewedProduct recentlyViewedProduct);       
    }
}
