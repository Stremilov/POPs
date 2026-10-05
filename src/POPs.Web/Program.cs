using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using POPs.Web;
using POPs.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Адрес API (можно поменять в wwwroot/appsettings.json)
var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5080/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<ApiClient>();
builder.Services.AddSingleton<AuthState>();

await builder.Build().RunAsync();
