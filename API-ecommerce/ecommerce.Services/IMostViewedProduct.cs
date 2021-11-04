using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IMostViewedProduct : IServices<MostViewedProduct>
    {
        Task<POJO> Save(MostViewedProduct mostViewedProduct);
    }
}
