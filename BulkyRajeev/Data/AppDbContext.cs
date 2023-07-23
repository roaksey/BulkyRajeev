﻿using BulkyRajeev.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyRajeev.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }
        public DbSet<Category> Categories { get;set; }

        //Seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                 new Category() { Id = 1, Name = "Action", DisplayOrder = 1 },
                 new Category() { Id = 2, Name = "Comedy", DisplayOrder = 2},
                 new Category() { Id = 3, Name = "Drama", DisplayOrder = 3}
                );
        }
    }
}
