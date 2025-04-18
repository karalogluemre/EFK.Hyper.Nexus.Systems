using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Commons.Application.Features.Queries.File
{
    public class DownloadFileQueryHandler(IConfiguration config) : IRequestHandler<DownloadFileQueryRequest, (Stream stream, string filename)>
    {
        private readonly IConfiguration config = config;

        public async Task<(Stream stream, string filename)> Handle(DownloadFileQueryRequest request, CancellationToken cancellationToken)
        {
            var client = new MongoClient(this.config["MongoSettings:ConnectionString"]);
            var database = client.GetDatabase(this.config["MongoSettings:DatabaseName"]);
            var gridFs = new GridFSBucket(database);

            var fileId = ObjectId.Parse(request.Id);
            try
            {
                var stream = await gridFs.OpenDownloadStreamAsync(fileId, cancellationToken: cancellationToken);
                var fileName = stream.FileInfo.Filename;

                return (stream, fileName);
            }
            catch (GridFSFileNotFoundException ex)
            {
                throw new FileNotFoundException($"Dosya bulunamadı: {request.Id}");
            }
        }
    }
}
