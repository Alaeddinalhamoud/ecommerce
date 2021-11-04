using ecommerce.Data;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IServices<T> where T : class
    {
        Task<T> Get(int? id);
        Task<IQueryable<T>> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        Task<POJO> Delete(int? id);
        //Just for test
        IQueryable<T> GetAllByPost(DataFiltter<T> dataFiltter);
    }
}
