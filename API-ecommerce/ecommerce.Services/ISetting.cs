using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ISetting : IServices<Setting>
    {
        Task<POJO> Save(Setting setting);
    }
}
