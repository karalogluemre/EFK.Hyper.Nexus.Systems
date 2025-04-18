using Commons.Application.Abstract.Dto.File;
using Commons.Application.Repositories.Queries;
using Commons.Domain.MongoFile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Commons.Persistence.Repositories.CrudRepositories.Queries
{
    public class MongoReadRepository<TDbContext> : IMongoReadRepository<TDbContext> where TDbContext : DbContext
    {
        private readonly IMongoDatabase database;
        private readonly GridFSBucket gridFs;
        readonly private IReadRepository<TDbContext, FileMetaData> readRepository;

        public MongoReadRepository(IConfiguration config, IReadRepository<TDbContext, FileMetaData> readRepository)
        {
            var client = new MongoClient(config["MongoSettings:ConnectionString"]);
            this.database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
            this.gridFs = new GridFSBucket(this.database);
            this.readRepository = readRepository;
        }

        public async Task<Stream> DownloadFileAsync(string objectId)
        {
            if (!ObjectId.TryParse(objectId, out var id))
                throw new ArgumentException("Geçersiz Mongo ObjectId", nameof(objectId));

            return await this.gridFs.OpenDownloadStreamAsync(id);
        }

        public async Task<PreviewFileQueryResponse> PreviewFileQueryResponse(string objectId)
        {
            var metaData = await this.readRepository.GetWhere(x => x.ReferenceId == Guid.Parse(objectId)).FirstOrDefaultAsync();
            var fileId = ObjectId.Parse(metaData.ObjectId.ToString());
            var stream = await this.gridFs.OpenDownloadStreamAsync(fileId);

            var contentType = stream.FileInfo.Metadata?.GetValue("ContentType")?.AsString
                              ?? GetMimeType(stream.FileInfo.Filename);

            return new PreviewFileQueryResponse
            {
                Stream = stream,
                ContentType = contentType,
                FileName = stream.FileInfo.Filename
            };
        }
        private static string GetMimeType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            return provider.TryGetContentType(fileName, out var contentType) ? contentType : "application/octet-stream";
        }

        public async Task<FilePreviewDto> FileByteResponse(string objectId)
        {
            var metaData = await this.readRepository
               .GetWhere(x => x.ReferenceId == Guid.Parse(objectId))
               .FirstOrDefaultAsync();

            if (metaData == null)
                return null;

            var fileId = ObjectId.Parse(metaData.ObjectId);
            var stream = await this.gridFs.OpenDownloadStreamAsync(fileId);

            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);

            return new FilePreviewDto
            {
                FileBytes = memory.ToArray(),
                FileName = stream.FileInfo.Filename,
                ContentType = stream.FileInfo.Metadata?["ContentType"]?.AsString ?? "application/octet-stream"
            };
        }
    }
}