using Commons.Application.Features.Commands.File;
using Commons.Application.Features.Queries.File;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsureNet.WebAPI.Controllers.File
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator mediator = mediator;

        [HttpPost("[action]")]
        public async Task<IActionResult> FileUpload([FromForm] UploadFileCommandRequest file)
        {
            var result = await this.mediator.Send(new UploadFileCommandRequest { File = file.File });
            return Ok(new { id = result });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> FileDownload([FromBody] DownloadFileQueryRequest request)
        {
            var result = await this.mediator.Send(request);
            return File(result.stream, "application/octet-stream", result.filename);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Preview([FromBody] PreviewFileQueryRequest request)
        {
            var result = await this.mediator.Send(request);
            return File(result.Stream, result.ContentType, result.FileName);
        }
    }
}
