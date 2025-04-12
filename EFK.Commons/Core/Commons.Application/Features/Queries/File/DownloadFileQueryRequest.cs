using MediatR;

namespace Commons.Application.Features.Queries.File
{
    public class DownloadFileQueryRequest : IRequest<(Stream stream, string filename)>
    {
        public string Id { get; set; } = string.Empty;
    }
}