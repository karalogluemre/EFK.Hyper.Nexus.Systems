using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Company.Remove
{
    public class RemoveCompanyCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }
}
