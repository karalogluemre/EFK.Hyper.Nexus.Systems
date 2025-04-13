using Commons.Application.Repositories.Queries;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Commons.Persistence.Repositories.CrudRepositories.Queries
{
    public class MongoReadRepository : IMongoReadRepository
    {
        private readonly IMongoDatabase database;
        private readonly GridFSBucket gridFs;

        public MongoReadRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoSettings:ConnectionString"]);
            this.database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
            this.gridFs = new GridFSBucket(this.database);
        }

        public async Task<Stream> DownloadFileAsync(string objectId)
        {
            if (!ObjectId.TryParse(objectId, out var id))
                throw new ArgumentException("Geçersiz Mongo ObjectId", nameof(objectId));

            return await this.gridFs.OpenDownloadStreamAsync(id);
        }
    }
}