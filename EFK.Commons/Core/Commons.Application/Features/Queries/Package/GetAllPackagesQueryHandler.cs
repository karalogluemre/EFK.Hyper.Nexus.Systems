using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Package
{
    public class GetAllPackagesQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticReadRepository,
      IRedisReadRepository redisReadRepository,
      IRedisWriteRepository redisWriteRepository
  ) : IRequestHandler<GetAllPackagesQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        private readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticSearchReadRepository = elasticReadRepository;
        private readonly IRedisReadRepository redisReadRepository = redisReadRepository;
        private readonly IRedisWriteRepository redisWriteRepository = redisWriteRepository;
        public async Task<BaseResponse> Handle(GetAllPackagesQueryRequest request, CancellationToken cancellationToken)
        {
            const string redisKey = "packages:all";

            var redisData = await this.redisReadRepository.GetAsync<List<Commons.Domain.Models.Packages.Package>>(redisKey);
            if (redisData != null && redisData.Any() || (redisData.Count != 0 ||redisData.Count <0 ))
            {
                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Veriler Redis üzerinden getirildi.",
                    Data = redisData.Where(p => p.IsDeleted != true).ToList()
                };
            }

            var allPackages = await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
            var filtered = allPackages.Where(p => p.IsDeleted != true).ToList();

            await this.redisWriteRepository.SetAsync(redisKey, filtered, TimeSpan.FromMinutes(10));

            return new BaseResponse
            {
                Succeeded = true,
                Message = "Veriler Elasticsearch üzerinden getirildi ve Redis’e yazıldı.",
                Data = filtered
            };
        }

    }
}
