using EFK.Commons.Config.ConfigModel;

namespace EFK.Commons.Config
{
    public class AppSecrets
    {
        public Dictionary<string, string> Admin { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AdminLog { get; set; } = new Dictionary<string, string>();
        public ElasticSearchSettings ElasticSearchSettings { get; set; }
        public RabbitMQSettings RabbitMQSettings { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public RedisSettings RedisSettings { get; set; }
        public MongoSettings MongoSettings { get; set; }
    }
}
