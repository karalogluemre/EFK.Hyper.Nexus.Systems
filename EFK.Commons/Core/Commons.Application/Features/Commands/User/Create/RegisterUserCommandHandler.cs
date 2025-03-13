using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using Commons.Domain.Models.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.User.Create
{
    public class RegisterUserCommandHandler<TDbContext>(
         IWriteRepository<TDbContext, AppUser> writeRepository,
         IPasswordHasher<AppUser> passwordHasher,
            IMapper mapper
        ) : IRequestHandler<RegisterUserCommandRequest, BaseResponse>
    where TDbContext : DbContext
    {
        private readonly IWriteRepository<TDbContext, AppUser> writeRepository = writeRepository;
        private readonly IPasswordHasher<AppUser> passwordHasher = passwordHasher;
        private readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = this.mapper.Map<AppUser>(request);
            user.PasswordHash = this.passwordHasher.HashPassword(user, request.Password);
            return await this.writeRepository.AddBulkAsync(new List<AppUser> { user });
        }
    }
}
