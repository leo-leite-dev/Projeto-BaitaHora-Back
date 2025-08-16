using AutoMapper;
using BaitaHora.Application.DTOs.Responses;
using BaitaHora.Application.DTOs.Users.Responses;
using BaitaHora.Domain.Entities.Commons.ValueObjects;
using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Application.Mapping.Profiles
{
    public sealed class UserResponseProfile : Profile
    {
        public UserResponseProfile()
        {
            CreateMap<Address, AddressResponse>()
                .ForMember(d => d.District, o => o.MapFrom(s => s.Neighborhood));

            CreateMap<User, UserResponse>()
                .ForMember(d => d.CreatedAtUtc, o => o.MapFrom(s => new DateTimeOffset(
                    DateTime.SpecifyKind(s.CreatedAt, DateTimeKind.Utc))))
                .ForMember(d => d.UpdatedAtUtc, o => o.MapFrom(s => s.UpdatedAt.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(s.UpdatedAt.Value, DateTimeKind.Utc))
                    : (DateTimeOffset?)null));

            CreateMap<UserProfile, UserProfileResponse>()
                .ForMember(d => d.CpfMasked, o => o.MapFrom(s => MaskCpf(s.CPF)))
                .ForMember(d => d.BirthDateUtc, o => o.MapFrom(s => s.BirthDate.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(s.BirthDate.Value, DateTimeKind.Utc))
                    : (DateTimeOffset?)null));
        }

        private static string? MaskCpf(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length < 11) return null;
            return $"***.***.{cpf[^5..^3]}-{cpf[^2..]}";
        }
    }
}