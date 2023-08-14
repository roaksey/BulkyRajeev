using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposiotry;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(AppDbContext db)
        {
            _db = db;
            //_dbCateogries == dbSet
            dbSet = _db.Set<T>();
            _db.Products.Include(x => x.Category).Include(y => y.CategoryId);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query = (tracked)?dbSet:dbSet.AsNoTracking();
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter = null,string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query.Where(filter);
            }
            if(!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
