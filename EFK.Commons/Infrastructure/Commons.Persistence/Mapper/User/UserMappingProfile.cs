using AutoMapper;
using Commons.Application.Features.Commands.User.Create;
using Commons.Domain.Models.User;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterUserCommandRequest, AppUser>();
    }
}
