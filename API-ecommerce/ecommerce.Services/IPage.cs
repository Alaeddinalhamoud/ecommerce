using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IPage : IServices<Page>
    {
        Task<POJO> Save(Page page);
    }
}
