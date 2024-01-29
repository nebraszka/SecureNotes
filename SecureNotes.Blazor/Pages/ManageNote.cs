using Microsoft.AspNetCore.Components;

namespace SelfAID.WebClient.Pages;

public partial class ManageNote : ComponentBase
{
    protected string Message = string.Empty;
    // protected GetEmotionDto emotion {get; set;} = new GetEmotionDto();

    [Parameter]
    public string Name { get; set; }

    // [Inject]
    // private IEmotionService emotionService { get; set; }

    [Inject]
    private NavigationManager navigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // // Update emotion
        // if(!string.IsNullOrEmpty(Name))
        // {
        //     var response = await emotionService.GetEmotionByName(Name);
        //     if(response == null)
        //     {
        //         Message = "Błąd pobierania emocji";
        //     }
        //     if(response.Success)
        //     {
        //         emotion = response.Data;
        //     }
        //     else
        //     {
        //         Message = $"Błąd pobierania emocji: {response.Message}";
        //     }
        // }
    }

    protected async void HandleValidRequest()
    {
        if (string.IsNullOrEmpty(Name))
        {
            // var response = await emotionService.AddEmotion(new AddEmotionDto { Name = emotion.Name, Description = emotion.Description });
            // if(response == null)
            // {
            //     Message = "Błąd dodawania emocji";
            //     StateHasChanged();
            // }
            // else
            // {
            //     if(response.Success)
            //     {
            //         GoToEmotions();
            //     }
            //     else
            //     {
            //         Message = $"Błąd dodawania emocji: {response.Message}";
            //         StateHasChanged();
            //     }
            // }
        }
        else
        {
            // var response = await emotionService.UpdateEmotion(new UpdateEmotionDto { Name = emotion.Name, Description = emotion.Description });
            // if(response == null)
            // {
            //     Message = "Błąd aktualizacji emocji";
            //     StateHasChanged();
            // }
            // else
            // {
            //     if(response.Success)
            //     {
            //         GoToEmotions();
            //     }
            //     else
            //     {
            //         Message = $"Błąd aktualizacji emocji: {response.Message}";
            //         StateHasChanged();
            //     }
            // }
        }
    }

    protected void HandleInvalidRequest()
    {
        Message = "Błąd walidacji danych";
        StateHasChanged();
    }

    protected void GoToEmotions()
    {
        navigationManager.NavigateTo("/emocje");
    }

    protected async Task DeleteEmotion()
    {
        // var response = await emotionService.DeleteEmotion(emotion.Name);
        // if(response == null)
        // {
        //     Message = "Błąd usuwania emocji";
        //     StateHasChanged();
        // }
        // else
        // {
        //     if(response.Success)
        //     {
        //         GoToEmotions();
        //     }
        //     else
        //     {
        //         Message = $"Błąd usuwania emocji: {response.Message}";
        //         StateHasChanged();
        //     }
        // }
    }
}