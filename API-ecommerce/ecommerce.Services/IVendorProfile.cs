using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IVendorProfile : IServices<VendorProfile>
    {
        Task<POJO> Save(VendorProfile vendorProfile);
    }
}