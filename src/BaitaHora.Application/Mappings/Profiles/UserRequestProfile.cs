using AutoMapper;
using BaitaHora.Application.DTOs.Requests;
using BaitaHora.Domain.Entities.Commons.ValueObjects;
using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Application.Mapping.Profiles
{
    public sealed class UserRequestProfile : Profile
    {
        public UserRequestProfile()
        {
            CreateMap<AddressRequest, Address>()
                .ConstructUsing(s =>
                    Address.Create(
                        s.Street,
                        s.Number,
                        s.District,
                        s.City,
                        s.State,
                        s.ZipCode,
                        string.IsNullOrWhiteSpace(s.Complement) ? null : s.Complement
                    )
                );

            CreateMap<UserRequest, UserProfile>()
                .ConstructUsing((src, ctx) =>
                {
                    var addr = ctx.Mapper.Map<Address>(src.Profile.Address);

                    return UserProfile.Create(
                        src.Profile.FullName,
                        src.Profile.CPF,
                        addr,
                        string.IsNullOrWhiteSpace(src.Profile.RG) ? null : src.Profile.RG,
                        src.Profile.BirthDate,
                        string.IsNullOrWhiteSpace(src.Profile.Phone) ? null : src.Profile.Phone,
                        string.IsNullOrWhiteSpace(src.Profile.ProfileImageUrl) ? null : src.Profile.ProfileImageUrl
                    );
                });
        }
    }
}