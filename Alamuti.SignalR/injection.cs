using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SignalR
{
    public static class Injection
    {
        public static IServiceCollection RegisterInfrastructerServices(
          this IServiceCollection service,
          IConfiguration configuration)
        {

            return service;
        }
    }
}
