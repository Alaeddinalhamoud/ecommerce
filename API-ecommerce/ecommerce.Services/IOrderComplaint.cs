using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrderComplaint : IServices<OrderComplaint>
    {
        Task<POJO> Save(OrderComplaint orderComplaint);
    }
}
