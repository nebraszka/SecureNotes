using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.SignalR.Protocol;
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

        public async Task<ServiceResponseWithoutData> ChangeNotePassword(Guid userId, ChangeNotePasswordRequestDto changeNotePasswordRequest)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.Id == changeNotePasswordRequest.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == changeNotePasswordRequest.NoteId);

                    if (note!.UserId == userId)
                    {
                        if (note.IsEncrypted)
                        {
                            if (string.IsNullOrEmpty(changeNotePasswordRequest.OldPassword))
                            {
                                return new ServiceResponseWithoutData
                                {
                                    Success = false,
                                    Message = "Hasło nie może być puste"
                                };
                            }

                            using (var hmac = new HMACSHA512(note.PasswordSalt!))
                            {
                                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changeNotePasswordRequest.OldPassword!));

                                for (int i = 0; i < computedHash.Length; i++)
                                {
                                    if (computedHash[i] != note.PasswordHash![i])
                                    {
                                        return new ServiceResponseWithoutData
                                        {
                                            Success = false,
                                            Message = "Niepoprawne hasło"
                                        };
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(changeNotePasswordRequest.NewPassword))
                        {
                            return new ServiceResponseWithoutData
                            {
                                Success = false,
                                Message = "Hasło nie może być puste"
                            };
                        }

                        using (var hmac = new HMACSHA512())
                        {
                            byte[] key = AesEncryption.CreateAesKeyFromPassword(changeNotePasswordRequest.NewPassword!, hmac.Key);
                            note.Content = AesEncryption.Encrypt(note.Content!, Convert.ToBase64String(key), note.Iv!);

                            note.IsEncrypted = true;
                            note.PasswordSalt = hmac.Key;
                            note.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changeNotePasswordRequest.NewPassword!));
                        }

                        await _context.SaveChangesAsync();

                        return new ServiceResponseWithoutData
                        {
                            Success = true,
                            Message = "Hasło zostało zmienione"
                        };
                    }

                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Nie masz uprawnień do zmiany hasła tej notatki"
                    };
                }
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }
            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        // TODO - clean code - extract methods and messages to constants
        // TESTED
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

        public async Task<ServiceResponseWithoutData> DecryptNote(Guid userId, DecryptNoteRequestDto decryptNoteRequestDto)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.Id == decryptNoteRequestDto.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == decryptNoteRequestDto.NoteId);

                    if (!note!.IsEncrypted)
                    {
                        return new ServiceResponseWithoutData
                        {
                            Success = false,
                            Message = "Notatka nie jest zaszyfrowana"
                        };
                    }

                    if (note!.UserId == userId)
                    {
                        if (string.IsNullOrEmpty(decryptNoteRequestDto.Password))
                        {
                            return new ServiceResponseWithoutData
                            {
                                Success = false,
                                Message = "Hasło nie może być puste"
                            };
                        }

                        using (var hmac = new HMACSHA512(note.PasswordSalt!))
                        {
                            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(decryptNoteRequestDto.Password!));

                            for (int i = 0; i < computedHash.Length; i++)
                            {
                                if (computedHash[i] != note.PasswordHash![i])
                                {
                                    return new ServiceResponseWithoutData
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

                            byte[] key = AesEncryption.CreateAesKeyFromPassword(decryptNoteRequestDto.Password!, note.PasswordSalt!);
                            note.Content = AesEncryption.Decrypt(note.Content!, Convert.ToBase64String(key), note.Iv!);

                            note.IsEncrypted = false;
                            note.PasswordSalt = null;
                            note.PasswordHash = null;
                        }

                        await _context.SaveChangesAsync();

                        return new ServiceResponseWithoutData
                        {
                            Success = true,
                            Message = "Notatka została odszyfrowana"
                        };
                    }

                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Nie masz uprawnień do odszyfrowania tej notatki"
                    };
                }

                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }

            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        // TESTED
        public async Task<ServiceResponseWithoutData> DeleteNote(Guid userId, DeleteNoteRequestDto deleteNoteRequestDto)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.Id == deleteNoteRequestDto.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == deleteNoteRequestDto.NoteId);

                    if (note!.UserId == userId)
                    {
                        if (note.IsEncrypted)
                        {
                            if (string.IsNullOrEmpty(deleteNoteRequestDto.Password))
                            {
                                return new ServiceResponseWithoutData
                                {
                                    Success = false,
                                    Message = "Hasło nie może być puste"
                                };
                            }

                            using (var hmac = new HMACSHA512(note.PasswordSalt!))
                            {
                                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(deleteNoteRequestDto.Password!));

                                for (int i = 0; i < computedHash.Length; i++)
                                {
                                    if (computedHash[i] != note.PasswordHash![i])
                                    {
                                        return new ServiceResponseWithoutData
                                        {
                                            Success = false,
                                            Message = "Niepoprawne hasło"
                                        };
                                    }
                                }
                            }
                        }

                        _context.Notes.Remove(note);
                        await _context.SaveChangesAsync();

                        return new ServiceResponseWithoutData
                        {
                            Success = true,
                            Message = "Notatka została usunięta"
                        };
                    }

                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Nie masz uprawnień do usunięcia tej notatki"
                    };
                }

                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }

            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        public async Task<ServiceResponseWithoutData> EncryptNote(Guid userId, EncryptNoteRequestDto encryptNoteRequestDto)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.Id == encryptNoteRequestDto.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == encryptNoteRequestDto.NoteId);

                    if (note!.IsEncrypted)
                    {
                        return new ServiceResponseWithoutData
                        {
                            Success = false,
                            Message = "Notatka jest już zaszyfrowana"
                        };
                    }

                    if (note!.UserId == userId)
                    {
                        if (string.IsNullOrEmpty(encryptNoteRequestDto.Password))
                        {
                            return new ServiceResponseWithoutData
                            {
                                Success = false,
                                Message = "Hasło nie może być puste"
                            };
                        }

                        using (var hmac = new HMACSHA512())
                        {
                            byte[] key = AesEncryption.CreateAesKeyFromPassword(encryptNoteRequestDto.Password!, hmac.Key);
                            note.Content = AesEncryption.Encrypt(note.Content!, Convert.ToBase64String(key), note.Iv!);

                            note.IsEncrypted = true;
                            note.PasswordSalt = hmac.Key;
                            note.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(encryptNoteRequestDto.Password!));
                        }

                        await _context.SaveChangesAsync();

                        return new ServiceResponseWithoutData
                        {
                            Success = true,
                            Message = "Notatka została zaszyfrowana"
                        };
                    }

                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Nie masz uprawnień do zaszyfrowania tej notatki"
                    };
                }

                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }

            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        // TESTED
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
                    Success = true,
                    Message = "Brak notatek dla tego użytkownika"
                };
            }

            return new ServiceResponse<List<GetNoteDto>>
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        // TESTED
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

        // TESTED
        public async Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid userId, GetNoteDetailsRequestDto getNoteDetailsRequestDto)
        {
            if (_context.Users.Any(u => u.UserId == userId))
            {
                if (_context.Notes.Any(n => n.Id == getNoteDetailsRequestDto.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == getNoteDetailsRequestDto.NoteId);

                    if (note!.UserId == userId || note.IsPublic)
                    {
                        var noteDetails = new GetNoteDetailsDto
                        {
                            Title = note.Title,
                            Content = note.Content,
                            CreationDate = note.CreationDate,
                            IsPublic = note.IsPublic,
                            IsEncrypted = note.IsEncrypted
                        };

                        if (note!.IsEncrypted)
                        {
                            if (string.IsNullOrEmpty(getNoteDetailsRequestDto.Password))
                            {
                                return new ServiceResponse<GetNoteDetailsDto>
                                {
                                    Success = false,
                                    Message = "Hasło nie może być puste"
                                };
                            }

                            using (var hmac = new HMACSHA512(note.PasswordSalt!))
                            {
                                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(getNoteDetailsRequestDto.Password!));

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

                                byte[] key = AesEncryption.CreateAesKeyFromPassword(getNoteDetailsRequestDto.Password!, note.PasswordSalt!);
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
                        Message = "Nie masz uprawnień do wyświetlenia tej notatki"
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

        public async Task<ServiceResponseWithoutData> MakeNotePublic(Guid userId, MakeNotePublicRequestDto makeNotePublicRequestDto)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.Id == makeNotePublicRequestDto.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == makeNotePublicRequestDto.NoteId);

                    if (note!.UserId == userId)
                    {
                        if (note.IsEncrypted)
                        {
                            if (string.IsNullOrEmpty(makeNotePublicRequestDto.Password))
                            {
                                return new ServiceResponseWithoutData
                                {
                                    Success = false,
                                    Message = "Hasło nie może być puste"
                                };
                            }

                            using (var hmac = new HMACSHA512(note.PasswordSalt!))
                            {
                                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(makeNotePublicRequestDto.Password!));

                                for (int i = 0; i < computedHash.Length; i++)
                                {
                                    if (computedHash[i] != note.PasswordHash![i])
                                    {
                                        return new ServiceResponseWithoutData
                                        {
                                            Success = false,
                                            Message = "Niepoprawne hasło"
                                        };
                                    }
                                }
                            }
                        }

                        note.IsPublic = true;

                        await _context.SaveChangesAsync();

                        return new ServiceResponseWithoutData
                        {
                            Success = true,
                            Message = "Notatka została udostępniona"
                        };
                    }

                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Nie masz uprawnień do udostępnienia tej notatki"
                    };
                }

                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }

            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }

        // Only title and content can be updated here
        public async Task<ServiceResponseWithoutData> UpdateNote(Guid userId, UpdateNoteDto updatedNote)
        {
            if (await _context.Users.AnyAsync(u => u.UserId == userId))
            {
                if (await _context.Notes.AnyAsync(n => n.Id == updatedNote.NoteId))
                {
                    var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == updatedNote.NoteId);

                    if (note!.UserId == userId)
                    {
                        if (note.IsEncrypted)
                        {
                            if (string.IsNullOrEmpty(updatedNote.Password))
                            {
                                return new ServiceResponseWithoutData
                                {
                                    Success = false,
                                    Message = "Hasło nie może być puste"
                                };
                            }

                            using (var hmac = new HMACSHA512(note.PasswordSalt!))
                            {
                                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(updatedNote.Password!));

                                for (int i = 0; i < computedHash.Length; i++)
                                {
                                    if (computedHash[i] != note.PasswordHash![i])
                                    {
                                        return new ServiceResponseWithoutData
                                        {
                                            Success = false,
                                            Message = "Niepoprawne hasło"
                                        };
                                    }
                                }
                            }
                        }

                        if (note.IsEncrypted)
                        {
                            if (string.IsNullOrEmpty(updatedNote.Password))
                            {
                                return new ServiceResponseWithoutData
                                {
                                    Success = false,
                                    Message = "Hasło nie może być puste"
                                };
                            }

                            // Check if password has matched
                            using (var hmac = new HMACSHA512(note.PasswordSalt!))
                            {
                                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(updatedNote.Password!));

                                for (int i = 0; i < computedHash.Length; i++)
                                {
                                    if (computedHash[i] != note.PasswordHash![i])
                                    {
                                        return new ServiceResponseWithoutData
                                        {
                                            Success = false,
                                            Message = "Niepoprawne hasło"
                                        };
                                    }
                                }

                                // Change note content to new encrypted content
                                using (var aes = Aes.Create())
                                {
                                    aes.IV = Convert.FromBase64String(note.Iv!);
                                }

                                byte[] key = AesEncryption.CreateAesKeyFromPassword(updatedNote.Password!, note.PasswordSalt!);
                                note.Content = AesEncryption.Encrypt(updatedNote.Content!, Convert.ToBase64String(key), note.Iv!);
                            }
                        }
                        else
                        {
                            note.Content = updatedNote.Content;
                        }

                        note.Title = updatedNote.Title;
                        note.CreationDate = DateTime.Now;

                        await _context.SaveChangesAsync();

                        return new ServiceResponseWithoutData
                        {
                            Success = true,
                            Message = "Notatka została zaktualizowana"
                        };
                    }

                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Nie masz uprawnień do aktualizacji tej notatki"
                    };
                }

                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Nie znaleziono notatki"
                };
            }

            return new ServiceResponseWithoutData
            {
                Success = false,
                Message = "Nie znaleziono użytkownika"
            };
        }
    }
}