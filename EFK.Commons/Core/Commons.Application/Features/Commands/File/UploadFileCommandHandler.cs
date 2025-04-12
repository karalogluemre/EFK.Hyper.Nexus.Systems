using Commons.Application.Repositories.Commands;
using MediatR;

namespace Commons.Application.Features.Commands.File
{
    public class UploadFileCommandHandler(IMongoWriteRepository mongoWriteRepository) : IRequestHandler<UploadFileCommandRequest, string>
    {
        private readonly IMongoWriteRepository mongoWriteRepository = mongoWriteRepository;

        public async Task<string> Handle(UploadFileCommandRequest request, CancellationToken cancellationToken)
        {
            var objectId = await this.mongoWriteRepository.UploadFileAsync(request.File);
            return objectId.ToString();
        }
    }
}
