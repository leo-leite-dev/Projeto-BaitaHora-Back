using AutoMapper;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.Mapping.Profiles
{
    public sealed class SchedulingResponseProfile : Profile
    {
        public SchedulingResponseProfile()
        {
            CreateMap<Schedule, ScheduleResponse>()
                .ForMember(d => d.CreatedAtUtc, o => o.MapFrom(s => new DateTimeOffset(DateTime.SpecifyKind(s.CreatedAtUtc, DateTimeKind.Utc))));

            CreateMap<Appointment, AppointmentResponse>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy.ToString()))
                .ForMember(d => d.StartsAtUtc, o => o.MapFrom(s => new DateTimeOffset(DateTime.SpecifyKind(s.StartsAtUtc, DateTimeKind.Utc))))
                .ForMember(d => d.EndsAtUtc, o => o.MapFrom(s => new DateTimeOffset(DateTime.SpecifyKind(s.EndsAtUtc, DateTimeKind.Utc))));

            CreateMap<ServiceCatalogItem, ServiceCatalogItemResponse>();
        }
    }
}