using AutoMapper;
using Commons.Application.Features.Commands.PackageMenu.Create;
using Commons.Domain.Models.Packages;

namespace Commons.Persistence.Mapper.Package
{
    public class PackageMenuMappingProfile : Profile
    {
        public PackageMenuMappingProfile()
        {
            CreateMap<CreatePackageMenuCommandRequest, PackageMenu>();
        }
    }
}
