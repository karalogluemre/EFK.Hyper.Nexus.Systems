using Commons.Application.Features.Commands.Branch.Create;
using Commons.Application.Features.Commands.Branch.Remove;
using Commons.Application.Features.Commands.Package.Remove;
using Commons.Application.Features.Queries.Branch;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.Branch
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllBranches()
        {
            var response = await this.mediator.Send(new GetAllBranchQueryRequest());
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBranch([FromForm] CreateBranchCommandRequest createBranchCommandHandler)
        {
            BaseResponse response = await this.mediator.Send(createBranchCommandHandler);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> DeletePackage([FromBody] RemoveBranchCommandRequest removeBranchCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(removeBranchCommandRequest);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> DeleteBulkAllBranches([FromBody] RemoveBulkAllBranchesCommandRequest deleteBulkAllBranchesCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(deleteBulkAllBranchesCommandRequest);
            return Ok(response);
        }
    }
}
