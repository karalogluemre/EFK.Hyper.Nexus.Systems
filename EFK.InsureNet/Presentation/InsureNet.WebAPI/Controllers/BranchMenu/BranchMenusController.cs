using Commons.Application.Features.Commands.BranchMenu.Create;
using Commons.Application.Features.Queries.BranchMenu;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.BranchMenu
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchMenusController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllBranchMenu( GetBranchMenusByCompanyIdQueryRequest getBranchMenusByCompanyIdQueryRequest)
        {
            var response = await this.mediator.Send(getBranchMenusByCompanyIdQueryRequest);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBranchMenus([FromForm] CreateBranchMenuCommandRequest request)
        {
            BaseResponse response = await mediator.Send(request);
            return Ok(response);
        }
    }
}
