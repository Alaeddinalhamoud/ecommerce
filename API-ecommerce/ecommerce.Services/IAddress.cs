using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IAddress : IServices<Address>
    {
        Task<POJO> Save(Address address);
        Task<POJO> UpdateIsDelete(Address entity);
    }
}
