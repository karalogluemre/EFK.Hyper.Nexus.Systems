using Commons.Application.Repositories.Commands;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class RabbitMQProducer : IRabbitMQProducer
     
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQProducer(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Publish(string jsonData, string entityName)
        {
            string queueName = entityName.ToLower();
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(jsonData);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}
