using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SecureNotes.API.Data;
using SecureNotes.API.Encryption;
using SecureNotes.API.Models;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.API.Services.Interfaces;
using SecureNotes.Shared.Models;

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
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (newNote.IsEncrypted && string.IsNullOrEmpty(newNote.Password))
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Hasło nie może być puste"
                    };
                }

                string? iv = null;
                byte[]? passwordHash = null;
                byte[]? passwordSalt = null;
                string? encryptedContent = null;

                if (newNote.IsEncrypted)
                {
                    using (var aes = Aes.Create())
                    {
                        aes.GenerateIV();
                        iv = Convert.ToBase64String(aes.IV);
                    }

                    using (var hmac = new HMACSHA512())
                    {
                        byte[] key = AesEncryption.CreateAesKeyFromPassword(newNote.Password!, hmac.Key);
                        encryptedContent = AesEncryption.Encrypt(newNote.Content!, Convert.ToBase64String(key), iv);

                        passwordSalt = hmac.Key;
                        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newNote.Password!));
                    }
                }

                var note = new Note
                {
                    UserId = userId,
                    Title = newNote.Title,
                    Content = encryptedContent ?? newNote.Content,
                    CreationDate = DateTime.Now,
                    IsPublic = newNote.IsPublic,
                    IsEncrypted = newNote.IsEncrypted,
                    Iv = iv,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };

                await _context.Notes.AddAsync(note);
                await _context.SaveChangesAsync();

                return new ServiceResponseWithoutData
                {
                    Success = true,
                    Message = "Notatka została dodana"
                };
            }

            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        public async Task<ServiceResponseWithoutData> DecryptNote(Guid userId, Guid noteId, DecryptNoteDto decryptedNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponseWithoutData> DeleteNote(Guid userId, Guid noteId, DeleteNoteDto deletedNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponseWithoutData> EncryptNote(Guid userId, Guid noteId, EncryptNoteDto encryptedNote)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes(Guid userId)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.UserId == userId))
                {
                    var notes = await _context.Notes.Where(n => n.UserId == userId).ToListAsync();
                    var notesDto = notes.Select(n => new GetNoteDto
                    {
                        NoteId = n.Id,
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
            if (await _context.Notes.AnyAsync(n => n.IsPublic))
            {
                var notes = await _context.Notes.Where(n => n.IsPublic).ToListAsync();
                var notesDto = notes.Select(n => new GetNoteDto
                {
                    NoteId = n.Id,
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
                Message = "Brak publicznych notatek"
            };
        }

        public async Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid userId, Guid noteId, string? password)
        {
            if (_context.Users.Any(u => u.UserId == userId))
            {
                if (_context.Notes.Any(n => n.Id == noteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId);

                    var noteDetails = new GetNoteDetailsDto
                    {
                        Title = note!.Title,
                        Content = note.Content,
                        CreationDate = note.CreationDate,
                        IsPublic = note.IsPublic,
                        IsEncrypted = note.IsEncrypted
                    };

                    if (note!.IsEncrypted)
                    {
                        if (string.IsNullOrEmpty(password))
                        {
                            return new ServiceResponse<GetNoteDetailsDto>
                            {
                                Success = false,
                                Message = "Hasło nie może być puste"
                            };
                        }

                        using (var hmac = new HMACSHA512(note.PasswordSalt!))
                        {
                            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password!));

                            for (int i = 0; i < computedHash.Length; i++)
                            {
                                if (computedHash[i] != note.PasswordHash![i])
                                {
                                    return new ServiceResponse<GetNoteDetailsDto>
                                    {
                                        Success = false,
                                        Message = "Niepoprawne hasło"
                                    };
                                }
                            }

                            using (var aes = Aes.Create())
                            {
                                aes.IV = Convert.FromBase64String(note.Iv!);
                            }

                            byte[] key = AesEncryption.CreateAesKeyFromPassword(password!, note.PasswordSalt!);
                            noteDetails.Content = AesEncryption.Decrypt(note.Content!, Convert.ToBase64String(key), note.Iv!);
                        }
                    }

                    return new ServiceResponse<GetNoteDetailsDto>
                    {
                        Success = true,
                        Data = noteDetails
                    };
                }

                return new ServiceResponse<GetNoteDetailsDto>
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }

            return new ServiceResponse<GetNoteDetailsDto>
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        public async Task<ServiceResponseWithoutData> UpdateNote(Guid userId, Guid noteId, UpdateNoteDto updatedNote)
        {
            throw new NotImplementedException();
        }
    }
}