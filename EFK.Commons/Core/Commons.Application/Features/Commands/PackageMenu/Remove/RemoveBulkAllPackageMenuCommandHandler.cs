using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.PackageMenu.Remove
{
    public class RemoveBulkAllPackageMenuCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> readRepository
) : IRequestHandler<RemoveBulkAllPackageMenuCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository = writeRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> readRepository = readRepository;
        public async Task<BaseResponse> Handle(RemoveBulkAllPackageMenuCommandRequest request, CancellationToken cancellationToken)
        {
            var entities = this.readRepository.GetWhere(pm => request.Ids.Contains(pm.PackageId.ToString())).ToList();

            if (!entities.Any())
            {
                return new BaseResponse { Succeeded = true, Message = "Silinecek kayıt bulunamadı." };
            }

            return await this.writeRepository.RemoveRangeAsync(entities);
        }
    }
}