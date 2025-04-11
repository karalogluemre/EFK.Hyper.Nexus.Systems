using AutoMapper;
using Commons.Application.Features.Commands.Company.Create;

namespace Commons.Persistence.Mapper.Company
{
    public class CompanyMappingProfile : Profile
    {
        public CompanyMappingProfile()
        {
            //CreateMap<CreateCompanyCommandRequest, Commons.Domain.Models.Companies.Company>();
            CreateMap<CreateCompanyCommandRequest, Commons.Domain.Models.Companies.Company>()
               .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
