using ecommerce.Data;
using ecommerce.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class Repository<T> : IServices<T> where T : class
    {
        protected readonly DbContext _context;
        internal DbSet<T> dbSet;
        public Repository(DbContext context)
        {
            _context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<POJO> Delete(int? id)
        {
            POJO model = new POJO();
            if (id == null)
            {
                model.flag = false;
                model.message = "does not exist.";
                return model;
            }
            T _T = await dbSet.FindAsync(id);
            if (_T != null)
            {
                try
                {
                    dbSet.Remove(_T);
                    await _context.SaveChangesAsync();
                    model.flag = true;
                    model.message = "Has been Deleted.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            else
            {
                model.flag = false;
                model.message = "does not exist.";
            }

            return model;
        }

        public async Task<T> Get(int? id)
        {
            T _T = null;
            if (id != null)
            {
                _T = await dbSet.FindAsync(id);
            }
            return _T;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<IQueryable<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

            }
            if (orderBy != null)
            {
                return orderBy(query).AsQueryable();
            }
            return query.AsQueryable();
        }

        public IQueryable<T> GetAllByPost(DataFiltter<T> dataFiltter)
        {
            IQueryable<T> query = dbSet;
            if (dataFiltter.filter != null)
            {
                query = query.Where(dataFiltter.filter);
            }
            if (dataFiltter.includeProperties != null)
            {
                foreach (var includeProperty in dataFiltter.includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

            }
            if (dataFiltter.orderBy != null)
            {
                return dataFiltter.orderBy(query).AsQueryable();
            }
            return query.AsQueryable();
        }
    }
}
