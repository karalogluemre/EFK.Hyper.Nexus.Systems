using Commons.Application.Features.Commands.PackageMenu.Create;
using Commons.Application.Features.Commands.PackageMenu.Remove;
using Commons.Application.Features.Commands.PackageMenu.Update;
using Commons.Application.Features.Queries.PackageMenu;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.PackageMenu
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageMenusController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllPackageMenu()
        {
            var response = await this.mediator.Send(new GetAllPackageMenuQueryRequest());
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePackageMenus([FromBody] CreatePackageMenuCommandRequest request)
        {
            BaseResponse response = await mediator.Send(request);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> RemovePackageMenu([FromBody] RemovePackageMenuCommandRequest request)
        {
            BaseResponse response = await this.mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveBulkAllPackageMenu([FromBody] RemoveBulkAllPackageMenuCommandRequest request)
        {
            BaseResponse response = await this.mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePackageMenu([FromBody] UpdatePackageMenuCommandRequest updatePackageMenuCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(updatePackageMenuCommandRequest);
            return Ok(response);
        }
    }
}