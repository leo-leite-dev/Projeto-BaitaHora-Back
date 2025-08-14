using AutoMapper;
using BaitaHora.Application.DTOs.Requests;
using BaitaHora.Application.DTOs.Requests.Company; 
using BaitaHora.Domain.Entities;                    
using BaitaHora.Domain.Entities.Users;             

namespace BaitaHora.Application.Mapping.Profiles
{
    public sealed class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<AddressRequest, Address>()
                .ForCtorParam("street", o => o.MapFrom(s => s.Street.Trim()))
                .ForCtorParam("number", o => o.MapFrom(s => s.Number.Trim()))
                .ForCtorParam("neighborhood", o => o.MapFrom(s => s.District.Trim()))
                .ForCtorParam("city", o => o.MapFrom(s => s.City.Trim()))
                .ForCtorParam("state", o => o.MapFrom(s => s.State.Trim()))
                .ForCtorParam("zipCode", o => o.MapFrom(s => s.ZipCode.Trim()))
                .ForCtorParam("complement", o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Complement) ? null : s.Complement.Trim()));

            CreateMap<CompanyRequest, Company>()
                .ConstructUsing((src, ctx) =>
                    new Company(
                        name: src.Name.Trim(),
                        address: ctx.Mapper.Map<Address>(src.Address),
                        document: string.IsNullOrWhiteSpace(src.Document) ? null : src.Document.Trim()
                    )
                );
        }
    }
}