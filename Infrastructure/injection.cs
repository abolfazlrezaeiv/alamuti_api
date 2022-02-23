using application.Interfaces.Data;
using application.Interfaces.repository;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

           
            service.AddScoped<IAdvertisementRepository, AdvertisementRepository>();

            service.AddScoped<AuthRepository>();
            service.AddScoped<MessageRepository>();
            return service;
        }
    }
}
