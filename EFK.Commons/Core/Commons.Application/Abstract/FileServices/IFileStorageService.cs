using Microsoft.AspNetCore.Http;

namespace Commons.Application.Abstract.FileServices
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string relativePath); // opsiyonel
    }
}
