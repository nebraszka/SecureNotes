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

        [HttpGet("all/{userId}"), Authorize]
        public async Task<ActionResult<ServiceResponse<List<GetNoteDto>>>> GetAllNotes([FromHeader] Guid userId)
        {
            var response = await _noteService.GetAllNotes(userId);

            return Ok(response);
        }

        [HttpGet("{noteId}"), AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<GetNoteDetailsDto>>> GetNoteDetails([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromQuery] string? password)
        {
            var response = await _noteService.GetNoteDetails(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> CreateNote([FromHeader] Guid userId, [FromBody] AddNoteDto newNote)
        {
            var response = await _noteService.CreateNote(userId, newNote);

            return Ok(response);
        }

        [HttpDelete("{noteId}"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> DeleteNote([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromQuery] string? password)
        {
            var response = await _noteService.DeleteNote(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost("{noteId}/encrypt"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> EncryptNote([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromQuery] string password)
        {
            var response = await _noteService.EncryptNote(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost("{noteId}/decrypt"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> DecryptNote([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromQuery] string password)
        {
            var response = await _noteService.DecryptNote(userId, noteId, password);

            return Ok(response);
        }

        [HttpPost("{noteId}/make-public"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> MakeNotePublic([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromQuery] string? password)
        {
            var response = await _noteService.MakeNotePublic(userId, noteId, password);

            return Ok(response);
        }

        [HttpPut("{noteId}"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> UpdateNote([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromBody] UpdateNoteDto updatedNote)
        {
            var response = await _noteService.UpdateNote(userId, noteId, updatedNote);

            return Ok(response);
        }

        [HttpPost("{noteId}/change-password"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> ChangeNotePassword([FromHeader] Guid userId, [FromRoute] Guid noteId, [FromQuery] string oldPassword, [FromQuery] string newPassword)
        {
            var response = await _noteService.ChangeNotePassword(userId, noteId, oldPassword, newPassword);

            return Ok(response);
        }
    }
}