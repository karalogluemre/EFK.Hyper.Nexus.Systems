using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using Commons.Domain.Models.Adreses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Commons.Application.Features.Commands.Adreses
{
    public class ProvinceAndDistrictCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Province> provinceWriteRepository,
    IWriteRepository<TDbContext, District> districtWriteRepository,
    IMapper mapper
) : IRequestHandler<ProvinceAndDistrictCommandRequest, BaseResponse>
    where TDbContext : DbContext
    {
        private readonly IWriteRepository<TDbContext, Province> provinceWriteRepository = provinceWriteRepository;
        private readonly IWriteRepository<TDbContext, District> districtWriteRepository = districtWriteRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> Handle(ProvinceAndDistrictCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var client = new HttpClient();
                var apiResponse = await client.GetFromJsonAsync<ProvinceApiResponse>("https://turkiyeapi.dev/api/v1/provinces");

                if (apiResponse?.Data is null)
                {
                    return new BaseResponse { Succeeded = false, Message = "API'den veri alınamadı." };
                }

                foreach (var province in apiResponse.Data)
                {
                    var provinceId = Guid.NewGuid(); // Kendimiz oluşturuyoruz

                    var newProvince = new Province
                    {
                        Id = provinceId,
                        ExternalId = province.Id, // API'den gelen int ID
                        Name = province.Name,
                        Area = province.Area
                    };

                    await this.provinceWriteRepository.AddOrUpdateBulkAsync(new List<Province> { newProvince });

                    foreach (var district in province.Districts)
                    {
                        var newDistrict = new District
                        {
                            Id = Guid.NewGuid(),
                            ExternalId = district.Id, // API'den gelen int ID
                            Name = district.Name,
                            ProvinceId = provinceId
                        };

                        await this.districtWriteRepository.AddOrUpdateBulkAsync(new List<District> { newDistrict });
                    }
                }

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "İller ve ilçeler başarıyla kaydedildi."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"İller ve ilçeler kayıt edilirken hata oluştu: {ex.Message}"
                };
            }
        }

        // İçeride kullanılan JSON model
        public class ProvinceApiResponse
        {
            public List<ProvinceDto> Data { get; set; } = new();
        }

        public class ProvinceDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Area { get; set; }
            public List<DistrictDto> Districts { get; set; } = new();
        }

        public class DistrictDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}