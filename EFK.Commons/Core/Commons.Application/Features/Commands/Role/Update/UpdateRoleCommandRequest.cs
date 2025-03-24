using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Role.Update
{
    public class UpdateRoleCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
