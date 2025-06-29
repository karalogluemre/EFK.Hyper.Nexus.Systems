using Commons.Application.Features.Commands.Adreses;
using Commons.Application.Features.Queries.Adresess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFK.System.WebAPI.Controllers.Adresess
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdresessController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> GetProvincesFromElastic()
        {
            var result = await this.mediator.Send(new GetAllProvincesQueryRequest());
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetDistinctById(GetDistinctByIdQueryRequest getDistinctByIdQueryRequest)
        {
            var result = await this.mediator.Send(getDistinctByIdQueryRequest);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProvincesFromElastic()
        {
            var result = await this.mediator.Send(new ProvinceAndDistrictCommandRequest());
            return Ok(result);
        }
    }
}
