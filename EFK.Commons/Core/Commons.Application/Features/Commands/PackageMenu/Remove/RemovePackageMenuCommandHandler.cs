using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.PackageMenu.Remove
{
    public class RemovePackageMenuCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> readRepository
) : IRequestHandler<RemovePackageMenuCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository = writeRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> readRepository = readRepository;
        public async Task<BaseResponse> Handle(RemovePackageMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out Guid id))
            {
                return new BaseResponse { Succeeded = false, Message = "Geçersiz ID formatı." };
            }

            var entity = await this.readRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return new BaseResponse { Succeeded = false, Message = "Kayıt bulunamadı." };
            }

            return await this.writeRepository.RemoveRangeAsync(new List<Commons.Domain.Models.Packages.PackageMenu> { entity });
        }
    }
}