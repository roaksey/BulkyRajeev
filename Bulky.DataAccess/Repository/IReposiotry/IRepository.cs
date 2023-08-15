using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IReposiotry
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter = null,string? includeProperties = null);
        T GetFirst(Expression<Func<T, bool>> filter, string? includeProperties = null,bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}
