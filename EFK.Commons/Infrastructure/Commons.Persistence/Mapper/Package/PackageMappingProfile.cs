using AutoMapper;
using Commons.Application.Features.Commands.Package.Create;
using Commons.Application.Features.Commands.Package.Update;

namespace Commons.Persistence.Mapper.Package
{
    public class PackageMappingProfile : Profile
    {
        public PackageMappingProfile()
        {
            CreateMap<CreatePackageCommandRequest, Commons.Domain.Models.Packages.Package>();
            CreateMap<UpdatePackageCommandRequest, Commons.Domain.Models.Packages.Package>();

        }
    }
}
