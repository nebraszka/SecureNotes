using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SecureNotes.Blazor;
using SecureNotes.Blazor.Services;
using SecureNotes.Blazor.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });

builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();