using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Repositories;
using BaitaHora.Infrastructure.Services;
using BaitaHora.Infrastructure.Services.Chat;

using BaitaHora.Application.Mapping.Profiles;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IServices.Auths;
using BaitaHora.Application.IServices.IChatbot;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Application.Services.Auths;
using BaitaHora.Application.Services.Chatbot;
using BaitaHora.Application.Services.Scheduling;
using BaitaHora.Application.IServices.Users;
using BaitaHora.Application.Services.Users;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Application.Services.Companies;
using BaitaHora.Application.Services.Companies.Queries;

namespace BaitaHora.Configurations.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ===== DB =====
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // ===== HttpContext (se CookieService/access depende disso) =====


            // ===== AutoMapper Profiles =====
            services.AddAutoMapper(
                typeof(AuthProfile).Assembly,
                typeof(UserRequestProfile).Assembly,
                typeof(CompanyProfile).Assembly,
                typeof(UserResponseProfile).Assembly,
                typeof(SchedulingResponseProfile).Assembly
            );

            // ===== Generic Repo =====
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // ===== REPOSITORIES =====
            // Users / Companies
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
            services.AddScoped<ICompanyPositionRepository, CompanyPositionRepository>();

            // Customers
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICompanyCustomerRepository, CompanyCustomerRepository>();
            services.AddScoped<ICompanyCustomerProfessionalRepository, CompanyCustomerProfessionalRepository>();

            // Scheduling
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IServiceCatalogItemRepository, ServiceCatalogItemRepository>();

            // ===== SERVICES =====
            // Core / Infra
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPasswordManager, PasswordManager>();

            // Users / Company
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompanyPermissionService, CompanyPermissionService>();
            services.AddScoped<ICompanyPositionService, CompanyPositionService>();
            services.AddScoped<ICompanyMemberService, CompanyMemberService>();

            // Auth
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IAuthService, AuthService>();

            // Scheduling
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IServiceCatalogService, ServiceCatalogService>();

            // Chatbot
            services.AddScoped<IChatbotQuickService, ChatbotQuickService>();
            services.AddSingleton<IChatSessionStore, InMemoryChatSessionStore>();

            services.AddScoped<ICompanyPositionQueries, CompanyPositionQueries>();

            return services;
        }
    }
}