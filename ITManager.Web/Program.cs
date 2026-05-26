using ITManager.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ITManager.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var baseAddress = string.IsNullOrWhiteSpace(apiBaseUrl)
    ? new Uri("https://localhost:7232")
    : new Uri(apiBaseUrl);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = baseAddress });
builder.Services.AddScoped<ISampleService, SampleService>();
builder.Services.AddScoped<ITareaService, TareaService>();

await builder.Build().RunAsync();
