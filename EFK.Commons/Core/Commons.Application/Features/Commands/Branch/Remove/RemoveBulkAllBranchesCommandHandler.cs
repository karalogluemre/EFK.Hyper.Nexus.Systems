using Commons.Application.Repositories.Common;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Branch.Remove
{
    public class RemoveBulkAllBranchesCommandHandler<TDbContext>(
     IUnitOfWork<TDbContext> unitOfWork
     ) : IRequestHandler<RemoveBulkAllBranchesCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IUnitOfWork<TDbContext> unitOfWork = unitOfWork;

        public async Task<BaseResponse> Handle(RemoveBulkAllBranchesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var results = new List<BaseResponse>();

                foreach (var id in request.Ids)
                {
                    var result = await this.unitOfWork.GetWriteRepository<Commons.Domain.Models.Branches.Branch>().DeleteAsync(Guid.Parse(id));
                    results.Add(result);
                }

                return new BaseResponse
                {
                    Succeeded = results.All(r => r.Succeeded),
                    Message = $"{results.Count} kayıt başarıyla silindi.",
                    Data = results
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Toplu silme sırasında hata oluştu: {ex.Message}" };
            }
        }
    }
}