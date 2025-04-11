using Commons.Application.Features.Commands.Adreses;
using Commons.Application.Features.Queries.Adresess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.Adresess
{
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
