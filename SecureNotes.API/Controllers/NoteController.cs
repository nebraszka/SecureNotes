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

        [HttpGet("all/{userId}"), Authorize]
        public async Task<ActionResult<ServiceResponse<List<GetNoteDto>>>> GetAllNotes(Guid userId)
        {
            var response = await _noteService.GetAllNotes(userId);

            return Ok(response);
        }
    }
}