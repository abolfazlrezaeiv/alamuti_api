using application.Interfaces.Data;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Data
{
    public class AlamutDbContext : IdentityDbContext, IAlamutDbContext
    {

        public AlamutDbContext(DbContextOptions<AlamutDbContext> options) : base(options)
        {
           
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Advertisement>().HasData(
        //        new Advertisement {Id=1, Title = "apple", Description = "stive jobs apple", Price = 11231 },
        //        new Advertisement { Id = 2, Title = "orage", Description = "ramsar porteghal", Price = 35 },
        //        new Advertisement { Id = 3, Title = "Pride", Description = "good for family", Price = 12321 });
        //}
        public DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    }

    public class AlamutDbContextFactory : IDesignTimeDbContextFactory<AlamutDbContext>
    {
       

        AlamutDbContext IDesignTimeDbContextFactory<AlamutDbContext>.CreateDbContext(string[] args)
        {
          
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();
            var optionsBuilder = new DbContextOptionsBuilder<AlamutDbContext>();

         
            var connectionString = configuration.GetConnectionString("Alamut");

            optionsBuilder.UseSqlServer(connectionString);

            return new AlamutDbContext(optionsBuilder.Options);
        }
    }
}


