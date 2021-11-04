using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IProductType : IServices<ProductType>
    {
        Task<POJO> Save(ProductType entity);
        Task<POJO> UpdateIsDelete(ProductType entity);
    }
}
