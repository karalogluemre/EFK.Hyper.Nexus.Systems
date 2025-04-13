namespace Commons.Application.Repositories.Queries
{
    public interface IMongoReadRepository
    {
        Task<Stream> DownloadFileAsync(string objectId);

    }
}
