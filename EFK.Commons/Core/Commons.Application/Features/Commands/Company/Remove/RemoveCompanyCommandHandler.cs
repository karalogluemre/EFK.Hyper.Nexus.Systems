using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Company.Remove
{
    public class RemoveCompanyCommandHandler<TDbContext>(
     IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository
 ) : IRequestHandler<RemoveCompanyCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository = writeRepository;
        public async Task<BaseResponse> Handle(RemoveCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            return await this.writeRepository.DeleteAsync(Guid.Parse(request.Id));
        }
    }
}
