using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IAlert : IServices<Alert>
    {
        Task<POJO> Save(Alert alert);
    }
}
