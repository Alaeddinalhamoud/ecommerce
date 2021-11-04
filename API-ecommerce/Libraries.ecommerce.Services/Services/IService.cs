
using ecommerce.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libraries.ecommerce.Services.Services
{
    public interface IService<T> where T : class
    {
        Task<T> Get(string id, string url, string token);
        Task<IQueryable<T>> GetAll(string url, string token);
        Task<string> GetByPost(T t, string url, string token);
        Task<POJO> Delete(int? id, string url, string token);
        Task<POJO> Post(T t, string url, string token);
        Task<POJO> PostRange(List<T> t, string url, string token);

        Task<string> GetAccessToken();
    }
}
