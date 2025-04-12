using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Company.Create
{
    public class CreateCompanyCommandHandler<TDbContext>(
        IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository,
        IMapper mapper
        ) : IRequestHandler<CreateCompanyCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        public async Task<BaseResponse> Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var company = mapper.Map<Commons.Domain.Models.Companies.Company>(request);
                return await writeRepository.AddOrUpdateBulkAsync(new List<Commons.Domain.Models.Companies.Company> { company });
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Firma kaydı sırasında hata: {ex.Message}"
                };
            }
        }
    }
}