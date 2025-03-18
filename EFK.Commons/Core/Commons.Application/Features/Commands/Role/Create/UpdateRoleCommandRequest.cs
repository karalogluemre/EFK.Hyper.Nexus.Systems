using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Role.Create
{
    public class UpdateRoleCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
