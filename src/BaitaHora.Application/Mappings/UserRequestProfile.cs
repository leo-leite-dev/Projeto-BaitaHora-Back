using AutoMapper;
using BaitaHora.Application.DTOs.Requests;
using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Application.Mapping.Profiles
{
    public sealed class UserRequestProfile : Profile
    {
        public UserRequestProfile()
        {
            CreateMap<AddressRequest, Address>()
                .ForCtorParam("street", o => o.MapFrom(s => s.Street.Trim()))
                .ForCtorParam("number", o => o.MapFrom(s => s.Number.Trim()))
                .ForCtorParam("neighborhood", o => o.MapFrom(s => s.District.Trim()))
                .ForCtorParam("city", o => o.MapFrom(s => s.City.Trim()))
                .ForCtorParam("state", o => o.MapFrom(s => s.State.Trim()))
                .ForCtorParam("zipCode", o => o.MapFrom(s => s.ZipCode.Trim()))
                .ForCtorParam("complement", o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Complement) ? null : s.Complement.Trim()));

            CreateMap<UserRequest, UserProfile>()
                .ConstructUsing((src, ctx) =>
                    new UserProfile(
                        fullName: src.Profile.FullName.Trim(),
                        cpf: src.Profile.CPF.Trim(),
                        rg: string.IsNullOrWhiteSpace(src.Profile.RG) ? null : src.Profile.RG.Trim(),
                        phone: string.IsNullOrWhiteSpace(src.Profile.Phone) ? null : src.Profile.Phone.Trim(),
                        address: ctx.Mapper.Map<Address>(src.Profile.Address)
                    )
                );
        }
    }
}