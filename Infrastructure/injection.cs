using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class Injection
    {
        public static IServiceCollection RegisterInfrastructerServices(
          this IServiceCollection service,
          IConfiguration configuration)
        {

            service.AddDbContext<AlamutDbContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("Alamut")));

           
            service.AddScoped<IRepository<Advertisement>, AdvertisementRepository>();

            service.AddScoped<AuthRepository>();
            return service;
        }
    }
}
