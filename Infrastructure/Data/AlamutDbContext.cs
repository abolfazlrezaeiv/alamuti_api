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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ChatGroup>()
            //    .HasMany(b => b.Messages)
            //    .WithOne(e => e.ChatGroup)
            //    .IsRequired();


            modelBuilder.Entity<ChatMessage>()
             .HasOne(p => p.ChatGroup)
             .WithMany(b => b.Messages)
              .HasForeignKey(p => p.ChatGroupId);

            modelBuilder.Entity<ChatGroup>()
             .Navigation(b => b.Messages)
             .UsePropertyAccessMode(PropertyAccessMode.Property);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ChatMessage> Messages{ get; set; }
        public  DbSet<ChatGroup>  ChatGroups { get; set; }
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


