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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5281/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecruitmentApiService>();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
