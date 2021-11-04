using ecommerce.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IProductSpecification:IServices<ProductSpecification>
    {
        Task<POJO> Save(ProductSpecification entity);
        Task<POJO> SaveRange(List<ProductSpecification> entity);
        Task<POJO> UpdateIsDelete(ProductSpecification entity);

    }
}
