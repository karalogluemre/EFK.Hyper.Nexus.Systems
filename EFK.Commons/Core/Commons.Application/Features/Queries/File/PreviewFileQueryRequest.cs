using Commons.Application.Abstract.Dto.File;
using Commons.Domain.MongoFile;
using MediatR;

namespace Commons.Application.Features.Queries.File
{
    public class PreviewFileQueryRequest : IRequest<PreviewFileQueryResponse>
    {
        public string Id { get; set; }
    }
}