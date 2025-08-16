using AutoMapper;
using BaitaHora.Application.DTOs.Requests;
using BaitaHora.Application.DTOs.Commands.Users;
using BaitaHora.Application.DTOs.Requests.Company;
using BaitaHora.Application.DTOs.Commands.Companies;
using BaitaHora.Application.DTOs.Commands.Commons;
using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Auth.Requests;
using BaitaHora.Domain.Entities.Users;
using BaitaHora.Domain.Entities.Commons.ValueObjects;

namespace BaitaHora.Application.Mapping.Profiles
{
    public sealed class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterOwnerRequest, RegisterOwnerWithCompanyCommand>()
                .ForCtorParam("User", opt => opt.MapFrom(src => src.User))
                .ForCtorParam("Company", opt => opt.MapFrom(src => src.Company));

            CreateMap<RegisterEmployeeRequest, RegisterEmployeeCommand>()
                .ForMember(d => d.CompanyId, o => o.MapFrom(s => s.CompanyId))
                .ForMember(d => d.User, o => o.MapFrom(s => s.User))
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role));

            CreateMap<LoginRequest, AuthenticateCommand>()
                .ConstructUsing(src => new AuthenticateCommand(src.Identifier, src.Password));

            CreateMap<UserRequest, UserInput>();
            CreateMap<UserProfileInput, UserProfile>();
            CreateMap<UserProfileRequest, UserProfileInput>();

            CreateMap<CompanyRequest, CreateCompanyCommand>();

            CreateMap<AddressRequest, AddressDto>();
            CreateMap<AddressDto, Address>();
        }
    }
}