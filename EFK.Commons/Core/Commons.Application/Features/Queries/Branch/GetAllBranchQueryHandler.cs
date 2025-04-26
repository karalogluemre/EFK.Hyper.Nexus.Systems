using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Branch
{
    public class GetAllBranchQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Branches.Branch> elasticReadRepository
  ) : IRequestHandler<GetAllBranchQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        private readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Branches.Branch> elasticSearchReadRepository = elasticReadRepository;
        public async Task<BaseResponse> Handle(GetAllBranchQueryRequest request, CancellationToken cancellationToken)
        {
            var allPackages = await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
            return new BaseResponse
            {
                Succeeded = true,
                Message = "Şubeler başarıyla getirildi.",
                Data = allPackages.Where(p => p.IsDeleted != true).ToList()
            };
        }
    }
}
