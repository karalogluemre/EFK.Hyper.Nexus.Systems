using Commons.Application.Abstract.Dto.File;
using Commons.Domain.MongoFile;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Queries
{
    public interface IMongoReadRepository<TDbContext> where TDbContext : DbContext
    {
        Task<Stream> DownloadFileAsync(string objectId);
        Task<PreviewFileQueryResponse> PreviewFileQueryResponse(string objectId);
        Task<FilePreviewDto> FileByteResponse (string objectId);
    }
}
