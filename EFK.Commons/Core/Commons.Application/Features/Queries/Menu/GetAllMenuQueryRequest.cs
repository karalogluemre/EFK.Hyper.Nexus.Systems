using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Queries.Menu
{
    public class GetAllMenuQueryRequest : IRequest<List<Commons.Domain.Models.Menus.Menu>>
    {
    }
}
