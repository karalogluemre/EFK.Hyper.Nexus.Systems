using Commons.Application.Repositories.Queries;
using Commons.Domain.MongoFile;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Commons.Application.Features.Queries.File
{
    public class PreviewFileQueryHandler<TDbContext> : IRequestHandler<PreviewFileQueryRequest, PreviewFileQueryResponse> where TDbContext : DbContext
    {
        private readonly GridFSBucket gridFs;
        readonly private IReadRepository<TDbContext,FileMetaData> readRepository;

        public PreviewFileQueryHandler(IConfiguration config, IReadRepository<TDbContext, FileMetaData> readRepository)
        {
            var client = new MongoClient(config["MongoSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
            this.gridFs = new GridFSBucket(database);
            this.readRepository = readRepository;

        }

        public async Task<PreviewFileQueryResponse> Handle(PreviewFileQueryRequest request, CancellationToken cancellationToken)
        {
            var metaData = await this.readRepository.GetWhere(x => x.ReferenceId == Guid.Parse(request.Id)).FirstOrDefaultAsync();
            var fileId = ObjectId.Parse(metaData.ObjectId.ToString());
            var stream = await this.gridFs.OpenDownloadStreamAsync(fileId, cancellationToken: cancellationToken);

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
    }
}
