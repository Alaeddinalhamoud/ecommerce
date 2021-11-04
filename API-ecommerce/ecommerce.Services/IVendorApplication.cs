using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IVendorApplication : IServices<VendorApplication>
    {
        Task<POJO> Save(VendorApplication vendorApplication);
    }
}
