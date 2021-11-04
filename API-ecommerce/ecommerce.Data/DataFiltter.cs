using System;
using System.Linq;
using System.Linq.Expressions;

namespace ecommerce.Data
{
   public class DataFiltter<T>
    {
     public Expression<Func<T, bool>> filter { get; set; }
     public Func<IQueryable<T>, IOrderedQueryable<T>> orderBy { get; set; } 
     public string includeProperties { get; set; }
    }
}
