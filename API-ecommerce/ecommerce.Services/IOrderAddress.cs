using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrderAddress : IServices<OrderAddress>
    {
        Task<POJO> Save(OrderAddress orderAddress);
    }
}
