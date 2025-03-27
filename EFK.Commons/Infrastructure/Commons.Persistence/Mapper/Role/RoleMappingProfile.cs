using AutoMapper;
using Commons.Application.Features.Commands.Role.Create;
using Commons.Application.Features.Commands.Role.Update;
using Commons.Domain.Models.Role;

namespace Commons.Persistence.Mapper.Role
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<CreateRoleCommandRequest, AppRole>()
                            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? Guid.NewGuid() : Guid.Parse(src.Id)));

            CreateMap<UpdateRoleCommandRequest, AppRole>();
        }
    }

}