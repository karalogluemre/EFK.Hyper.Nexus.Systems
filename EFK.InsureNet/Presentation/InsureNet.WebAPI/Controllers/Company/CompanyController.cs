using Commons.Application.Features.Commands.Company.Create;
using Commons.Application.Features.Queries.Company;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.Company
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(IMediator mediator) : ControllerBase
    {
        readonly IMediator mediator = mediator;
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllCompany()
        {
            var response = await this.mediator.Send(new GetAllCompaniesQueryRequest());
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommandRequest createCompanyCommandRequest)
        {
            BaseResponse response = await this.mediator.Send(createCompanyCommandRequest);
            return Ok(response);
        }

    }
}
