using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Company.Remove
{
    public class RemoveAllCompanyCommandRequest : IRequest<BaseResponse>
    {
        public List<string> PackageIds { get; set; } = new();
    }
}
