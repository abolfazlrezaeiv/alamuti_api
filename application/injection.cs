using application.AutoMapper;
using application.AutoMapper.Chat;
using application.Interfaces;
using application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace application
{
    public static class Injection
    {
        public static IServiceCollection RegisterApplicationServices(
             this IServiceCollection service,
             IConfiguration configuration)
        {
            service.AddAutoMapper(typeof(AdvertisementProfile),typeof(ChatProfile));
            service.AddScoped<IOTPSevice , OTPServices>();
            return service;
        }
    }
}
