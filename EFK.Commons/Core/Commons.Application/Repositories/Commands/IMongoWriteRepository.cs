using Commons.Domain.MongoFile;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Commands
{
    public interface IMongoWriteRepository
    {
        Task<string> UploadFileAsync<TEntity>(FileMetaData fileMetaData);
        Task<bool> DeleteFileAsync<TEntity>(string objectId);
    }
}
