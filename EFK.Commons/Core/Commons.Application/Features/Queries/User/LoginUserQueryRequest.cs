using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Queries.User
{
    public class LoginUserQueryRequest : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
