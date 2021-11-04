using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
   public interface IFAQ : IServices<FAQ>
    {
        Task<POJO> Save(FAQ fAQ); 
    }
}
