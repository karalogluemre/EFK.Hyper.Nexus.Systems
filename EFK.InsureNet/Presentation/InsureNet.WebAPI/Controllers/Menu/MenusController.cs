using Commons.Application.Features.Commands.Menu.Create;
using Commons.Application.Features.Queries.Menu;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.Menu
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllMenu()
        {
            var response = await this.mediator.Send(new GetAllMenuQueryRequest());
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MenuUpload(CreateMenuCommandRequest  createMenuCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(createMenuCommandRequest);
            return Ok(response);
        }
    }
}
