using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using Commons.Domain.Models.Role;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Role.Create
{
    public class CreateRoleCommandHandler<TDbContext>(
        IWriteRepository<TDbContext,AppRole> writeRepository,
         IMapper mapper) : IRequestHandler<CreateRoleCommandRequest, BaseResponse>
    where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext , AppRole> writeRepository = writeRepository;
        readonly private IMapper mapper = mapper;
        public async Task<BaseResponse> Handle(CreateRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var role = this.mapper.Map<AppRole>(request);
            return await this.writeRepository.AddOrUpdateBulkAsync(new List<AppRole> { role });
        }
    }
}
