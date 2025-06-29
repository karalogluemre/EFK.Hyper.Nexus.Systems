using Commons.Application.Features.Commands.Package.Create;
using Commons.Application.Features.Commands.Package.Remove;
using Commons.Application.Features.Commands.Package.Update;
using Commons.Application.Features.Queries.Package;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EFK.System.WebAPI.Controllers.Package
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllPackage()
        {
            var response = await this.mediator.Send(new GetAllPackagesQueryRequest());
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetIdPackage([FromBody] GetPackageByIdQueryRequest getPackageByIdQueryRequest)
        {
            var response = await this.mediator.Send(getPackageByIdQueryRequest);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePackage([FromForm] CreatePackageCommandRequest createPackageCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(createPackageCommandRequest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePackage([FromBody] UpdatePackageCommandRequest updatePackageCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(updatePackageCommandRequest);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> DeletePackage([FromBody] DeletePackageCommandRequest deletePackageCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(deletePackageCommandRequest);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> DeleteBulkAllPackages([FromBody] DeleteBulkAllPackageCommandRequest deleteBulkAllPackageCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(deleteBulkAllPackageCommandRequest);
            return Ok(response);
        }

    }
}
