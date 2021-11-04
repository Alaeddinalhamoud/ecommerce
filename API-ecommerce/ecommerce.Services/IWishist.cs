using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IWishist:IServices<Wishlist>
    {
        Task<POJO> Save(Wishlist wishlist);
    }
}