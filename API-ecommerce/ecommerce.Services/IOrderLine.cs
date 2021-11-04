using ecommerce.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrderLine : IServices<OrderLine>
    {
        Task<POJO> Save(OrderLine orderLine);
        Task<POJO> SaveRange(List<OrderLine> entity);
    }
}
