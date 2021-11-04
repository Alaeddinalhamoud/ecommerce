using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IShippingCountry : IServices<ShippingCountry>
    {
        Task<POJO> Save(ShippingCountry shippingCountry);
    }
}
