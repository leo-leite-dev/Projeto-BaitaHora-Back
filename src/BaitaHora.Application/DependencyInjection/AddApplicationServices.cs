using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BaitaHora.Application.Mapping.Profiles;
using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Auth.Validator;

namespace BaitaHora.Configurations.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(RegisterOwnerWithCompanyCommand).Assembly));

            services.AddValidatorsFromAssembly(typeof(RegisterOwnerWithCompanyValidator).Assembly);

            services.AddAutoMapper(typeof(AuthProfile).Assembly);

            return services;
        }
    }
}
