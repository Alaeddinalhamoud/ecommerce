using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrder:IServices<Order>
    {
        Task<POJO> Save(Order order); 
    }
}
