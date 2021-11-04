using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
   public interface IMedia : IServices<Media>
    {
        Task<POJO> Save(Media media);
        Task<POJO> UpdateIsDelete(Media entity);
    }
}
