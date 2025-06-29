using Commons.Application.Features.Commands.User.Create;
using Commons.Application.Features.Queries.User;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EFK.System.WebAPI.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginUser(LoginUserQueryRequest loginUserQueryRequest)
        {
            BaseResponse response = await this.mediator.Send(loginUserQueryRequest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterUser(RegisterUserCommandRequest registerUserCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(registerUserCommandRequest);
            return Ok(response);
        }
      
    }

}