using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Package.Remove
{
    public class DeletePackageCommandHandler<TDbContext>(
     IWriteRepository<TDbContext, Commons.Domain.Models.Packages.Package> writeRepository
 ) : IRequestHandler<DeletePackageCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IWriteRepository<TDbContext, Commons.Domain.Models.Packages.Package> writeRepository = writeRepository;
        public async Task<BaseResponse> Handle(DeletePackageCommandRequest request, CancellationToken cancellationToken)
        {
            return await writeRepository.DeleteAsync(Guid.Parse(request.Id));
        }
    }
}
