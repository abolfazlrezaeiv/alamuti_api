using application.Interfaces.repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace application
{
    public static class Injection
    {
        public static IServiceCollection RegisterApplicationServices(
             this IServiceCollection service,
             IConfiguration configuration)
        {

            return service;
        }
    }
}
