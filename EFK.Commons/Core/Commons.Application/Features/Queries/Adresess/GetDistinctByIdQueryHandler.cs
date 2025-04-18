using AutoMapper;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using Commons.Domain.Models.Adreses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Commons.Application.Abstract.Dto.AdresesDto;

namespace Commons.Application.Features.Queries.Adresess
{
    public class GetDistinctByIdQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, District> elasticReadRepository,
      IMapper mapper

  ) : IRequestHandler<GetDistinctByIdQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IElasticSearchReadRepository<TDbContext, District> elasticReadRepository = elasticReadRepository;
        readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(GetDistinctByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var allDistricts = await elasticReadRepository.GetAllFromElasticSearchAsync();
            if(request.Id == "string" ||request.Id.Length<0 || request.Id == null)
            {
                return new BaseResponse
                {
                    Succeeded = true,
                    Data = new List<DistrictDto>(),
                    Message = "İller ve ilçeler Elastic üzerinden listelendi."
                };
            }
            var filtered = allDistricts
                .Where(d => d.ProvinceId == Guid.Parse(request.Id))
                .ToList();

            var dtoList = mapper.Map<List<DistrictDto>>(filtered);
            return new BaseResponse
            {
                Succeeded = true,
                Data = dtoList,
                Message = "İller ve ilçeler Elastic üzerinden listelendi."
            };
        }
    }

}
