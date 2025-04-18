using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Company.Remove
{
    public class RemoveAllCompanyCommandHandler<TDbContext>(
         IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository
     ) : IRequestHandler<RemoveAllCompanyCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        private readonly IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository = writeRepository;

        public async Task<BaseResponse> Handle(RemoveAllCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var results = new List<BaseResponse>();

                foreach (var packageId in request.PackageIds)
                {
                    var result = await this.writeRepository.DeleteAsync(Guid.Parse(packageId));
                    results.Add(result);
                }

                return new BaseResponse
                {
                    Succeeded = results.All(r => r.Succeeded),
                    Message = $"{results.Count} kayıt başarıyla silindi.",
                    Data = results
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Toplu silme sırasında hata oluştu: {ex.Message}" };
            }
        }
    }
}