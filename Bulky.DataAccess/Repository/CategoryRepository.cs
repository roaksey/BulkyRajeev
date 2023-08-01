using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDbContext _db;
        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }      

        public void Update(Category obj)
        {
            _db.Update(obj);
        }
    }
}
