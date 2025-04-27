using Commons.Application.Features.Commands.Company.Remove;
using Commons.Application.Repositories.Common;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Branch.Remove
{
    public class RemoveBranchCommandHandler<TDbContext>(
     IUnitOfWork<TDbContext> unitOfWork
 ) : IRequestHandler<RemoveBranchCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        IUnitOfWork<TDbContext> unitOfWork = unitOfWork;
        public async Task<BaseResponse> Handle(RemoveBranchCommandRequest request, CancellationToken cancellationToken)
        {
            var result = await this.unitOfWork
                .GetWriteRepository<Commons.Domain.Models.Branches.Branch>()
                .DeleteAsync(Guid.Parse(request.Id));

            if (result.Succeeded)
            {
                await this.unitOfWork.CommitAsync();
            }

            return result;
        }
    }
}
