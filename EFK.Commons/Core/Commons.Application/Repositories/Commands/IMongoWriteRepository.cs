using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace Commons.Application.Repositories.Commands
{
    public interface IMongoWriteRepository
    {
        Task<ObjectId> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string objectId);
    }
}
