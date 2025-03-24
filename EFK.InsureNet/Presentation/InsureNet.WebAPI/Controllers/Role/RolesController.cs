using Commons.Application.Features.Commands.Role.Create;
using Commons.Application.Features.Commands.Role.Update;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.Role
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateRole(CreateRoleCommandRequest createRoleCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(createRoleCommandRequest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateRole(UpdateRoleCommandRequest updateRoleCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(updateRoleCommandRequest);
            return Ok(response);
        }
    }
}
