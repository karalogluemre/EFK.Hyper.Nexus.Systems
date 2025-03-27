using Commons.Application.Repositories.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQConsumer<TDbContext, TEntity> : BackgroundService
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMQConsumer(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _queueName = typeof(TEntity).Name.ToLower();

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
            UserName = configuration["RabbitMQ:Username"],
            Password = configuration["RabbitMQ:Password"]
        };

         _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var elasticSearchRepository = scope.ServiceProvider.GetRequiredService<IElasticSearchWriteRepository<TDbContext, TEntity>>();

                var entity = JsonConvert.DeserializeObject<TEntity>(message);
                if (entity != null)
                {
                    await elasticSearchRepository.AddToElasticSearchAsync(entity);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
