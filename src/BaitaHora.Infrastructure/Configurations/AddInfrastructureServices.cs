using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Repositories;
using BaitaHora.Infrastructure.Services;
using BaitaHora.Application.Mapping.Profiles;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.IAuth;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Application.Services.Auths;
using BaitaHora.Application.Services.Scheduling;
using BaitaHora.Application.IServices.Auths;
using BaitaHora.Application.IServices;
using BaitaHora.Application.IServices.Auth;
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
                typeof(UserResponseProfile).Assembly,
                typeof(SchedulingResponseProfile).Assembly
            );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();

            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IServiceCatalogItemRepository, ServiceCatalogItemRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IPasswordManager, PasswordManager>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICookieService, CookieService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IServiceCatalogService, ServiceCatalogService>();

            return services;
        }
    }
}