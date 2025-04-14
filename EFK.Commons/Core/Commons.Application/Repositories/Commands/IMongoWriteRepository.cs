using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Commands
{
    public interface IMongoWriteRepository
    {
        Task<string> UploadFileAsync(IFormFile file, Guid referenceId,string tag,string referanceName);
        Task DeleteFileAsync(string objectId);
    }
}
