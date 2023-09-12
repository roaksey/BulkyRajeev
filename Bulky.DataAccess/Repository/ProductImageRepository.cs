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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private AppDbContext _db;
        public ProductImageRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

     
        public void Update(ProductImage product)
        {
            _db.ProductImages.Update(product);
        }
    }
}
