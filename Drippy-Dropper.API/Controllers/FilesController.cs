using Drippy_Dropper.API.Configurations.Drippy_Dropper.API.Configurations;
using Drippy_Dropper.API.Functions.Commands.FileCommands;
using Drippy_Dropper.API.Functions.Queries.FileQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Drippy_Dropper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StorageSettings _storageSettings;

        public FilesController(IMediator mediator, StorageSettings storageSettings)
        {
            _mediator = mediator;
            _storageSettings = storageSettings;
        }

        [HttpPost("Add-File")]
        public async Task<IActionResult> AddFile([FromForm] AddFileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("Update-File")]
        public async Task<IActionResult> UpdateFile([FromBody] UpdateFileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("Delete-File/{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid fileId, [FromBody] DeleteFileCommand command)
        {
            command.FileId = fileId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("Get-File-Details")]
        public async Task<IActionResult> GetFileDetails([FromQuery] GetFileQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Get-Files-By-User")]
        public async Task<IActionResult> GetFilesByUser([FromQuery] GetFilesByUserQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Download-File")]
        public async Task<IActionResult> DownloadFile([FromQuery] GetFileDownloadQuery query)
        {
            var fileDownload = await _mediator.Send(query);

            return File(fileDownload.Content, fileDownload.ContentType, fileDownload.Name);
        }

    }
}
