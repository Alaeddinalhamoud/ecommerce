using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IProduct:IServices<Product>
    {
        Task<POJO> Save(Product product);
        Task<POJO> UpdateIsDelete(Product entity);
      //  Task<IQueryable<ProductDetail>> ProductDetail();
    }
}
