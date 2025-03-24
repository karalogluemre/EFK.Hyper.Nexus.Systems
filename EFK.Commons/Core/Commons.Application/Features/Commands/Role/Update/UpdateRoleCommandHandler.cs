using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models.Role;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Role.Update
{
    public class UpdateRoleCommandHandler<TDbContext>(
        IWriteRepository<TDbContext, AppRole> writeRepository,
         IMapper mapper) : IRequestHandler<UpdateRoleCommandRequest, BaseResponse>
    where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, AppRole> writeRepository = writeRepository;
        readonly private IMapper mapper = mapper;
        public async Task<BaseResponse> Handle(UpdateRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var role = mapper.Map<AppRole>(request);
            return await writeRepository.UpdateBulkAsync(new List<AppRole> { role });
        }
    }
}
