using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.IAuth;
using BaitaHora.Application.Services.Auths;
using BaitaHora.Infrastructure.Repositories;
using BaitaHora.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using BaitaHora.Application.Mapping.Profiles;
using BaitaHora.Application.IServices;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Application.IServices.Auths;


namespace BaitaHora.Configurations.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(
                typeof(UserRequestProfile).Assembly,
                typeof(CompanyProfile).Assembly,
                typeof(UserResponseProfile).Assembly
            );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();

            // UoW (se estiver usando)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IPasswordManager, PasswordManager>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICookieService, CookieService>();

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}