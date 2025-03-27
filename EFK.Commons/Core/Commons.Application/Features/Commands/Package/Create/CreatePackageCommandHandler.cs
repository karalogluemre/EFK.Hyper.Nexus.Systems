using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Package.Create
{
    public class CreatePackageCommandHandler<TDbContext>(
     IWriteRepository<TDbContext, Commons.Domain.Models.Packages.Package> writeRepository,
     IMapper mapper
 ) : IRequestHandler<CreatePackageCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IWriteRepository<TDbContext, Commons.Domain.Models.Packages.Package> writeRepository = writeRepository;
        readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreatePackageCommandRequest request, CancellationToken cancellationToken)
        {
            var package = this.mapper.Map<Commons.Domain.Models.Packages.Package>(request);
            return await this.writeRepository.AddBulkAsync(new List<Commons.Domain.Models.Packages.Package> { package });
        }
    }
}
