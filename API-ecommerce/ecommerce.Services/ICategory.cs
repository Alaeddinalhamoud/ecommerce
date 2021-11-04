using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ICategory:IServices<Category>
    {
        Task<POJO> Save(Category category);
        Task<POJO> UpdateIsDelete(Category entity);
    }
}
