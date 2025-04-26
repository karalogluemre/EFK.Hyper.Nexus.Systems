using AutoMapper;
using Commons.Application.Repositories.Common;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Package.Create
{
    public class CreatePackageCommandHandler<TDbContext>(
     IUnitOfWork<TDbContext> unitOfWork,
     IMapper mapper
 ) : IRequestHandler<CreatePackageCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IUnitOfWork<TDbContext> unitOfWork = unitOfWork;
        readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreatePackageCommandRequest request, CancellationToken cancellationToken)
        {
            var package = this.mapper.Map<Commons.Domain.Models.Packages.Package>(request);

            var result = await this.unitOfWork
                .GetWriteRepository<Commons.Domain.Models.Packages.Package>()
                .AddOrUpdateBulkAsync(new List<Commons.Domain.Models.Packages.Package> { package });

            if (result.Succeeded)
            {
                await this.unitOfWork.CommitAsync();
            }

            return result;
        }
    }
}
