using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
   public interface IBrand : IServices<Brand>
    {
        Task<POJO> Save(Brand brand);
        Task<POJO> UpdateIsDelete(Brand entity);
    }
}
