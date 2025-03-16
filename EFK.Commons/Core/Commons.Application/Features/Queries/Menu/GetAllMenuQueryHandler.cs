using Commons.Application.Repositories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Menu
{
    public class GetAllMenuQueryHandler
    <TDbContext>(
        IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> elasticSearchReadRepository,
        IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> readRepository
        ) : IRequestHandler<GetAllMenuQueryRequest, List<Commons.Domain.Models.Menus.Menu>> where TDbContext : DbContext
    {
        readonly IElasticSearchReadRepository<TDbContext, Domain.Models.Menus.Menu> elasticSearchReadRepository = elasticSearchReadRepository;
        readonly IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> readRepository = readRepository;

        public async Task<List<Commons.Domain.Models.Menus.Menu>> Handle(GetAllMenuQueryRequest request, CancellationToken cancellationToken)
        {
            return await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
        }
    }
}
