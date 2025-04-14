using Commons.Application.Repositories.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.File
{
    public class UploadFileCommandHandler<TDbContext>(IMongoWriteRepository mongoWriteRepository) : IRequestHandler<UploadFileCommandRequest, string> where TDbContext : DbContext
    {
        private readonly IMongoWriteRepository mongoWriteRepository = mongoWriteRepository;

        public async Task<string> Handle(UploadFileCommandRequest request, CancellationToken cancellationToken)
        {
            var objectId = await this.mongoWriteRepository.UploadFileAsync(request.File,request.Id,"","");
            return objectId.ToString();
        }
    }
}
