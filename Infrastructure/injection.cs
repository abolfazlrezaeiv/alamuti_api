using application.Interfaces.Data;
using Infrastructure.Data;
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
            {
                options.UseSqlServer(configuration.GetConnectionString("AlamutDbConttection"));
            });
            service.AddScoped<IAlamutDbContext>(options => options.GetService<AlamutDbContext>());
            return service;
        }
    }
}
