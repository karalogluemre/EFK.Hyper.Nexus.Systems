using AutoMapper;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using Commons.Domain.Models.Adreses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Commons.Application.Abstract.Dto.AdresesDto;

namespace Commons.Application.Features.Queries.Adresess
{
    public class GetAllProvincesQueryHandler<TDbContext>(
         IElasticSearchReadRepository<TDbContext, Province> provinceElasticRepo,
    IElasticSearchReadRepository<TDbContext, District> districtElasticRepo,
    IMapper mapper)
    : IRequestHandler<GetAllProvincesQueryRequest, BaseResponse>
    where TDbContext : DbContext
    {
        public async Task<BaseResponse> Handle(GetAllProvincesQueryRequest request, CancellationToken cancellationToken)
        {
            // 1. Province verilerini al
            var provinces = await provinceElasticRepo.GetAllFromElasticSearchAsync();
            if (provinces == null || !provinces.Any())
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = "İl verileri alınamadı."
                };
            }

            // 2. District verilerini al
            var districts = await districtElasticRepo.GetAllFromElasticSearchAsync();
            if (districts == null || !districts.Any())
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = "İlçe verileri alınamadı."
                };
            }

            // 3. Eşleştir
            foreach (var province in provinces)
            {
                province.Districts = districts
                    .Where(d => d.ProvinceId == province.Id)
                    .ToList();
            }

            // 4. DTO'ya dönüştür
            var dtoList = mapper.Map<List<ProvinceWithDistrictsDto>>(provinces);

            return new BaseResponse
            {
                Succeeded = true,
                Data = dtoList,
                Message = "İller ve ilçeler Elastic üzerinden listelendi."
            };
        }
    }
}