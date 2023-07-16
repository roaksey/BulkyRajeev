using BulkyRajeev.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyRajeev.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }
        public DbSet<Category> Categories { get;set; }
    }
}
