using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recruitement.Ui.Blazor;
using Recruitement.Ui.Blazor.Services;
using Recruitement.Ui.Blazor.Handlers;
using Recruitement.Ui.Blazor.Auth;
using Microsoft.AspNetCore.Components.Authorization;

using Recruitement.Ui.Blazor.ApiClients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp =>
{
    var js = sp.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    var handler = sp.GetRequiredService<JwtAuthorizationMessageHandler>();
    return new HttpClient(handler) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
});

await builder.Build().RunAsync();
// Register typed API client for Candidates
builder.Services.AddHttpClient<ICandidateApiClient, CandidateApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/api/candidates");
});
