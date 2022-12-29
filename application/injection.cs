using application.AutoMapper;
using application.AutoMapper.Chat;
using application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace application
{
    public static class Injection
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAutoMapper(typeof(AdvertisementProfile),typeof(ChatProfile));
            return service;
        }
    }
}
