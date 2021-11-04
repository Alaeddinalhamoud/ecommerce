using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IReview:IServices<Review>
    {
        Task<POJO> Save(Review review);
        Task<POJO> UpdateIsDelete(Review entity);
    }
}
