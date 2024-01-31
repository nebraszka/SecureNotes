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

        private Guid userId => Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

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
            var response = await _noteService.GetAllNotes(userId);

            return Ok(response);
        }

        [HttpPost("details"), Authorize]
        public async Task<ActionResult<ServiceResponse<GetNoteDetailsDto>>> GetNoteDetails([FromBody] GetNoteDetailsRequestDto getNoteDetailsRequest)
        {
            var response = await _noteService.GetNoteDetails(userId, getNoteDetailsRequest);

            return Ok(response);
        }

        [HttpPost("create"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> CreateNote([FromBody] AddNoteDto newNote)
        {
            var response = await _noteService.CreateNote(userId, newNote);

            return Ok(response);
        }

        [HttpPost("delete"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> DeleteNote([FromBody] DeleteNoteRequestDto deleteNoteRequest)
        {
            var response = await _noteService.DeleteNote(userId, deleteNoteRequest);

            return Ok(response);
        }

        [HttpPost("encrypt"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> EncryptNote([FromBody] EncryptNoteRequestDto encryptNoteRequest)
        {
            var response = await _noteService.EncryptNote(userId, encryptNoteRequest);

            return Ok(response);
        }

        [HttpPost("decrypt"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> DecryptNote([FromBody] DecryptNoteRequestDto decryptNoteRequest)
        {
            var response = await _noteService.DecryptNote(userId, decryptNoteRequest);

            return Ok(response);
        }

        [HttpPost("make-public"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> MakeNotePublic([FromBody] MakeNotePublicRequestDto makeNotePublicRequest)
        {
            var response = await _noteService.MakeNotePublic(userId, makeNotePublicRequest);

            return Ok(response);
        }

        [HttpPost("make-private"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> MakeNotePrivate([FromBody] MakeNotePrivateRequestDto makeNotePrivateRequest)
        {
            var response = await _noteService.MakeNotePrivate(userId, makeNotePrivateRequest);

            return Ok(response);
        }

        [HttpPut, Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> UpdateNote([FromBody] UpdateNoteDto updatedNote)
        {
            var response = await _noteService.UpdateNote(userId, updatedNote);

            return Ok(response);
        }

        [HttpPost("change-password"), Authorize]
        public async Task<ActionResult<ServiceResponseWithoutData>> ChangeNotePassword([FromBody] ChangeNotePasswordRequestDto changeNotePasswordRequest)
        {
            var response = await _noteService.ChangeNotePassword(userId, changeNotePasswordRequest);

            return Ok(response);
        }
    }
}