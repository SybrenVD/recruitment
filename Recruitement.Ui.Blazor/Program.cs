using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recruitement.Ui.Blazor;
using Recruitement.Ui.Blazor.ApiClients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register typed API client for Candidates
builder.Services.AddHttpClient<ICandidateApiClient, CandidateApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/api/candidates");
});

await builder.Build().RunAsync();
