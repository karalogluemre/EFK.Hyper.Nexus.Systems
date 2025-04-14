using AutoMapper;
using Commons.Application.Abstract.Dto.Company;
using Commons.Application.Features.Commands.Company.Create;

namespace Commons.Persistence.Mapper.Company
{
    public class CompanyMappingProfile : Profile
    {
        public CompanyMappingProfile()
        {

            CreateMap<CreateCompanyCommandRequest, Commons.Domain.Models.Companies.Company>()
               .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // Company → CompanyDto
            CreateMap<Commons.Domain.Models.Companies.Company, CompanyDto>()
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore()); // Mongo'dan çekilecek

            // CompanyDto → Company (create/update için)
            CreateMap<CompanyDto, Commons.Domain.Models.Companies.Company>()
                .ForMember(dest => dest.CompanyFiles, opt => opt.Ignore()) // logo dosyası ayrı eklenir
                .ForMember(dest => dest.Branches, opt => opt.Ignore());
        }

    }
}
