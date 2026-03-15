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
            services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();

            // Services
            services.AddScoped<IInvestmentTypeService, InvestmentTypeService>();
            services.AddScoped<IFamilyMemberService, FamilyMemberService>();
            return services;
        }
    }
}
