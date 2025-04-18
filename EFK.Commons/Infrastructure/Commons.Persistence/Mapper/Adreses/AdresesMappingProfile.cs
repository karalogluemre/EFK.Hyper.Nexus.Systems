using AutoMapper;
using Commons.Domain.Models.Adreses;
using static Commons.Application.Abstract.Dto.AdresesDto;

namespace Commons.Persistence.Mapper.Adreses
{
    public class AdresesMappingProfile : Profile
    {
        public AdresesMappingProfile()
        {
            CreateMap<Province, ProvinceWithDistrictsDto>()
                 .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<District, DistrictDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId));


        }
    }
}