using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureNotes.API.Models;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.API.Services.Interfaces;

namespace SecureNotes.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet("public"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<List<GetNoteDto>>>> GetAllPublicNotes()
        {
            var response = await _noteService.GetAllPublicNotes();

            return Ok(response);
        }

        [HttpGet("all"), Authorize]
        public async Task<ActionResult<ServiceResponse<List<GetNoteDto>>>> GetAllNotes()
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.GetAllNotes(userId);

            return Ok(response);
        }

        [HttpGet("{noteId}"), Authorize]
        public async Task<ActionResult<ServiceResponse<GetNoteDetailsDto>>> GetNoteDetails([FromRoute] Guid noteId, [FromQuery] string? password)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.GetNoteDetails(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> CreateNote([FromBody] AddNoteDto newNote)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.CreateNote(userId, newNote);

            return Ok(response);
        }

        [HttpDelete("{noteId}"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> DeleteNote([FromRoute] Guid noteId, [FromQuery] string? password)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.DeleteNote(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost("{noteId}/encrypt"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> EncryptNote([FromRoute] Guid noteId, [FromQuery] string password)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.EncryptNote(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost("{noteId}/decrypt"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> DecryptNote([FromRoute] Guid noteId, [FromQuery] string password)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.DecryptNote(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost("{noteId}/make-public"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> MakeNotePublic([FromRoute] Guid noteId, [FromQuery] string? password)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.MakeNotePublic(userId, noteId, password);

            return Ok(response);
        }

        [HttpPut("{noteId}"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> UpdateNote([FromRoute] Guid noteId, [FromBody] UpdateNoteDto updatedNote)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var response = await _noteService.UpdateNote(userId, noteId, updatedNote);

            return Ok(response);
        }

        [HttpPost("{noteId}/change-password"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> ChangeNotePassword([FromRoute] Guid noteId, [FromQuery] string oldPassword, [FromQuery] string newPassword)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            
            var response = await _noteService.ChangeNotePassword(userId, noteId, oldPassword, newPassword);

            return Ok(response);
        }
    }
}