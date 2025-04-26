using AutoMapper;
using Commons.Application.Features.Commands.Branch.Create;
using Commons.Application.Features.Commands.Company.Create;

namespace Commons.Persistence.Mapper.Branch
{
    public class BranchMappingProfile : Profile
    {
        public BranchMappingProfile()
        {
            CreateMap<CreateBranchCommandRequest, Commons.Domain.Models.Branches.Branch>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
