using EFK.Commons.Config.ConfigModel;
using EFK.System.Persistence.Injection.Config.AppByteService;
using System.Text.Json;

namespace EFK.Commons.Config
{
    public class AppSettingsConfiguration
    {
        private readonly IAesService aesService;

        public AppSettingsConfiguration() {
            // DI yoksa, manuel oluştur
            aesService = new AesService(); // IAesService içinde başka bağımlılık yoksa bu YETERLİDİR
        }
        public async Task<AppSecrets> InitializeConfigsAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Config file not found: {filePath}");

            var jsonString = await File.ReadAllTextAsync(filePath);
            var records = JsonSerializer.Deserialize<List<ConfigRecord>>(jsonString);
            if (records == null)
                throw new Exception("Config file format is invalid.");

            var dict = new Dictionary<string, JsonElement>();
            foreach (var rec in records)
            {
                var decryptedJson = aesService.Decrypt(rec.Hash);

                var startIndex = decryptedJson.IndexOf('{');
                if (startIndex == -1)
                    throw new Exception("Decrypted json format is invalid: " + decryptedJson);

                var jsonPart = decryptedJson.Substring(startIndex);

                using var doc = JsonDocument.Parse(jsonPart);
                dict[rec.Name] = doc.RootElement.Clone();
            }

            var configs = new Dictionary<string, JsonElement>(dict, StringComparer.OrdinalIgnoreCase);

            var appSecrets = new AppSecrets();
            appSecrets.Admin =
                JsonSerializer.Deserialize<Dictionary<string, string>>(configs["Admin"].GetRawText())!;
            appSecrets.AdminLog =
                JsonSerializer.Deserialize<Dictionary<string, string>>(configs["AdminLog"].GetRawText())!;
          
            appSecrets.ElasticSearchSettings= JsonSerializer.Deserialize<ElasticSearchSettings>(configs["ElasticSearch"].GetRawText())!;
           
            appSecrets.RabbitMQSettings =
                JsonSerializer.Deserialize<RabbitMQSettings>(configs["RabbitMQ"].GetRawText())!;

         
            appSecrets.JwtSettings =
                JsonSerializer.Deserialize<JwtSettings>(configs["Jwt"].GetRawText())!;

            appSecrets.MongoSettings =
                JsonSerializer.Deserialize<MongoSettings>(configs["MongoSettings"].GetRawText())!;

            appSecrets.RedisSettings =
                JsonSerializer.Deserialize<RedisSettings>(configs["RedisSettings"].GetRawText())!;

        
            return appSecrets;
        }
    }
}
