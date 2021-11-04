using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IVendorBank : IServices<VendorBank>
    {
        Task<POJO> Save(VendorBank vendorBank);
    }
}