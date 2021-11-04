using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
   public interface IVendorMedia : IServices<VendorMedia>
    {
        Task<POJO> Save(VendorMedia media); 
    }
}