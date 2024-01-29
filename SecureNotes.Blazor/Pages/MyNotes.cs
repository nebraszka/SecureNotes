using Microsoft.AspNetCore.Components;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.Blazor.Services;

namespace SecureNotes.Blazor.Pages
{
    public partial class MyNotes : ComponentBase
    {
        [Inject]
        private INoteService? noteService { get; set; }
        public List<GetNoteDto>? notes { get; set; }
        public string notesListMessage = "";
        public string notesListClass = "";
        public string addNoteStatusMessage = "";
        public string addNoteStatusClass = "";

        protected override async Task OnInitializedAsync()
        {
            var response = await noteService!.GetAllNotes();

            if (response != null)
            {
                if (response.Success)
                {
                    notes = response.Data;
                }
                else
                {
                    notes = new List<GetNoteDto>();
                    notesListMessage = $"{response.Message}";
                    notesListClass = "alert-danger";
                }
            }
            else
            {
                notes = new List<GetNoteDto>();
                notesListMessage = "Błąd pobierania danych";
                notesListClass = "alert-danger";
            }
        }
    }
}