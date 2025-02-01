using Drippy_Dropper.API.Functions.Commands.FolderCommands;
using Drippy_Dropper.API.Functions.Queries.FolderQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Drippy_Dropper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoldersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Add-Folder")]
        public async Task<IActionResult> AddFolder([FromBody] AddFolderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("Update-Folder")]
        public async Task<IActionResult> UpdateFolder([FromBody] UpdateFolderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("Delete-Folder/{folderId}")]
        public async Task<IActionResult> DeleteFolder(Guid folderId, [FromBody] DeleteFolderCommand command)
        {
            command.FolderId = folderId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("Get-Folder-Contents")]
        public async Task<IActionResult> GetFolderContents([FromQuery] GetFolderContentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //[HttpGet("Get-All-Folders")]
        //public async Task<IActionResult> GetAllFolders([FromQuery] GetAllFoldersQuery query)
        //{
        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}

        [HttpGet("Get-Folders-By-User")]
        public async Task<IActionResult> GetFoldersByUser([FromQuery] GetFoldersByUserQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
