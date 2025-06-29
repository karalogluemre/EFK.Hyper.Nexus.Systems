using Commons.Application.Repositories.Commands;
using Commons.Domain.MongoFile;
using EFK.Commons.Config;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using WatchDog;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class MongoWriteRepository : IMongoWriteRepository
    {
        private readonly IMongoDatabase database;
        private readonly GridFSBucket gridFs;

        public MongoWriteRepository()
        {
            var configInitializer = new AppSettingsConfiguration();
            var appSecrets = configInitializer.InitializeConfigsAsync().GetAwaiter().GetResult();
            var client = new MongoClient(appSecrets.MongoSettings.ConnectionString);
            this.database = client.GetDatabase(appSecrets.MongoSettings.DatabaseName);
            this.gridFs = new GridFSBucket(this.database);
        }
        public async Task<string> UploadFileAsync<TEntity>(FileMetaData fileMetaData)
        {
            try
            {
                WatchLogger.Log($" Yükleme işlemi başlatıldı. Dosya Adı: {fileMetaData.FileName}, ReferansId: {fileMetaData.ReferenceId}");

                fileMetaData.ObjectId = Guid.NewGuid().ToString();

                var collection = database.GetCollection<FileMetaData>(typeof(TEntity).Name);
                await collection.InsertOneAsync(fileMetaData);

                WatchLogger.Log($" Dosya başarıyla yüklendi. FileId: {fileMetaData.ObjectId}");

                return fileMetaData.ReferenceId.ToString();
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Dosya yüklenirken bir hata oluştu. Dosya Adı: {fileMetaData.FileName}, ReferansId: {fileMetaData.ReferenceId}, Hata: {ex.Message}", ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync<TEntity>(string objectId)
        {
            try
            {
                WatchLogger.Log($"Dosya silme işlemi başlatıldı. ObjectId: {objectId}");

                if (!ObjectId.TryParse(objectId, out var id))
                {
                    WatchLogger.Log($"Geçersiz ObjectId formatı: {objectId}");
                    return false;
                }

                var collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
                var filter = Builders<TEntity>.Filter.Eq("_id", id);

                var deleteResult = await collection.DeleteOneAsync(filter);

                if (deleteResult.DeletedCount > 0)
                {
                    WatchLogger.Log($"Kayıt başarıyla silindi. ObjectId: {objectId}");
                    return true;
                }
                else
                {
                    WatchLogger.Log($"Kayıt bulunamadı veya silinemedi. ObjectId: {objectId}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($"Dosya silinirken hata oluştu. ObjectId: {objectId}, Hata: {ex.Message}", ex.ToString());
                throw;
            }
        }

    }
}