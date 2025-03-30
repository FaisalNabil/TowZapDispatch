using AutoMapper;
using Dispatch.Application.DTOs.Auth;
using Dispatch.Domain.Entities;

namespace Dispatch.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, LoginResponseDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        }
    }
}
