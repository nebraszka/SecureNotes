using Microsoft.EntityFrameworkCore;
using SecureNotes.API.Data;
using SecureNotes.API.Models;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.API.Services.Interfaces;

namespace SecureNotes.API.Services
{
    public class NoteService : INoteService
    {
        private readonly DataContext _context;

        public NoteService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponseWithoutData> CreateNote(Guid userId, AddNoteDto newNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponseWithoutData> DecryptNote(Guid noteId, DecryptNoteDto decryptedNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponseWithoutData> DeleteNote(Guid noteId, DeleteNoteDto deletedNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponseWithoutData> EncryptNote(Guid noteId, EncryptNoteDto encryptedNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes(Guid userId)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if(await _context.Notes.AnyAsync(n => n.UserId == userId))
                {
                    var notes = await _context.Notes.Where(n => n.UserId == userId).ToListAsync();
                    var notesDto = notes.Select(n => new GetNoteDto
                    {
                        Title = n.Title,
                        CreationDate = n.CreationDate,
                        IsPublic = n.IsPublic,
                        IsEncrypted = n.IsEncrypted
                    }).ToList();

                    return new ServiceResponse<List<GetNoteDto>>
                    {
                        Success = true,
                        Data = notesDto
                    };
                }

                return new ServiceResponse<List<GetNoteDto>>
                {
                    Success = false,
                    Message = "Brak notatek dla tego użytkownika"
                };
            }
            
            return new ServiceResponse<List<GetNoteDto>>
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

    public async Task<ServiceResponse<List<GetNoteDto>>> GetAllPublicNotes()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid noteId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponseWithoutData> UpdateNote(Guid noteId, UpdateNoteDto updatedNote)
    {
        throw new NotImplementedException();
    }
}
}