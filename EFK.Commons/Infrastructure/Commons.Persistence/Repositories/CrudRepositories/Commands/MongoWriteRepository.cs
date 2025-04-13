using Commons.Application.Repositories.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class MongoWriteRepository : IMongoWriteRepository
    {
        private readonly IMongoDatabase database;
        private readonly GridFSBucket gridFs;

        public MongoWriteRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoSettings:ConnectionString"]);
            this.database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
            this.gridFs = new GridFSBucket(this.database);
        }

        public async Task<ObjectId> UploadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var fileId = await this.gridFs.UploadFromStreamAsync(file.FileName, stream);
            return fileId;
        }
        public async Task DeleteFileAsync(string objectId)
        {
            try
            {
                if (ObjectId.TryParse(objectId, out var id))
                {
                    await this.gridFs.DeleteAsync(id);
                }
            }
            catch (GridFSFileNotFoundException)
            {
                // dosya zaten yoksa problem değil
            }
        }
    }
}
