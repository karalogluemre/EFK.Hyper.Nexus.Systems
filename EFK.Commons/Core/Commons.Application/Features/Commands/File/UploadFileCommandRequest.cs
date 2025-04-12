using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Commons.Application.Features.Commands.File
{
    public class UploadFileCommandRequest : IRequest<string>
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
