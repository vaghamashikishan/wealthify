using System;
using wealthify.Repositories;
using wealthify.Repositories.Interfaces;
using wealthify.Services;
using wealthify.Services.Interfaces;

namespace wealthify.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            // Repositories
            services.AddScoped<IInvestmentTypeRepository, InvestmentTypeRepository>();

            // Services
            services.AddScoped<IInvestmentTypeService, InvestmentTypeService>();
            return services;
        }
    }
}
