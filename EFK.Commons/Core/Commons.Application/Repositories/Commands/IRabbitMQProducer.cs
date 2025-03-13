namespace Commons.Application.Repositories.Commands
{
    public interface IRabbitMQProducer
    {
        void Publish(string jsonData,string entityName);
    }
}
