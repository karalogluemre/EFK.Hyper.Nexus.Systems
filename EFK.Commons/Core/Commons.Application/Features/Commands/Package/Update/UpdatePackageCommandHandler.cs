using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Package.Update
{
    public class UpdatePackageCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Commons.Domain.Models.Packages.Package> writeRepository,
    IMapper mapper
    ) : IRequestHandler<UpdatePackageCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IWriteRepository<TDbContext, Commons.Domain.Models.Packages.Package> writeRepository = writeRepository;
        readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(UpdatePackageCommandRequest request, CancellationToken cancellationToken)
        {
            var package = mapper.Map<Commons.Domain.Models.Packages.Package>(request);
            return await this.writeRepository.UpdateBulkAsync(new List<Commons.Domain.Models.Packages.Package> { package });
        }
    }
}