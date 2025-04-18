using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Queries.Adresess
{
    public class GetDistinctByIdQueryRequest : IRequest<BaseResponse>
    {
        public string? Id { get; set; }
    }
}
