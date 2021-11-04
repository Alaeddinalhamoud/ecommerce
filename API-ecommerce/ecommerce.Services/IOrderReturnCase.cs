using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrderReturnCase : IServices<OrderReturnCase>
    {
        Task<POJO> Save(OrderReturnCase orderReturnCase);
    }
}
