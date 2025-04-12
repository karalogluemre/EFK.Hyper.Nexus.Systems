using Commons.Domain.MongoFile;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Commons.Application.Features.Queries.File
{
    public class PreviewFileQueryHandler : IRequestHandler<PreviewFileQueryRequest, PreviewFileQueryResponse>
    {
        private readonly GridFSBucket gridFs;

        public PreviewFileQueryHandler(IConfiguration config)
        {
            var client = new MongoClient(config["MongoSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
            this.gridFs = new GridFSBucket(database);
        }

        public async Task<PreviewFileQueryResponse> Handle(PreviewFileQueryRequest request, CancellationToken cancellationToken)
        {
            var fileId = ObjectId.Parse(request.Id);
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
