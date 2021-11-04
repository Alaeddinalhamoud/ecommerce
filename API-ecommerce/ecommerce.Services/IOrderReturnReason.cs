using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrderReturnReason : IServices<OrderReturnReason>
    {
        Task<POJO> Save(OrderReturnReason orderReturnReason);
    }
}
