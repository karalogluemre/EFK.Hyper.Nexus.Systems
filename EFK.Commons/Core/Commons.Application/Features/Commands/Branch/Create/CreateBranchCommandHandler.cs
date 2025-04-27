using AutoMapper;
using Commons.Application.Repositories.Common;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Branch.Create
{
    public class CreateBranchCommandHandler<TDbContext>(
     IUnitOfWork<TDbContext> unitOfWork,
     IMapper mapper
 ) : IRequestHandler<CreateBranchCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IUnitOfWork<TDbContext> unitOfWork = unitOfWork;
        readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreateBranchCommandRequest request, CancellationToken cancellationToken)
        {
            var package = this.mapper.Map<Commons.Domain.Models.Branches.Branch>(request);

            var result = await this.unitOfWork
                .GetWriteRepository<Commons.Domain.Models.Branches.Branch>()
                .AddOrUpdateBulkAsync(new List<Commons.Domain.Models.Branches.Branch> { package });

            if (result.Succeeded)
            {
                await this.unitOfWork.CommitAsync();
            }

            return result;
        }
    }
}
