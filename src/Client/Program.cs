using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HsaLedger.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var url = string.Empty;

if (builder.HostEnvironment.IsDevelopment())
{
    url = "http://localhost:8081";
    //url = "http://localhost:5000";
}
else
{
    // TODO: Add production url
    url = "http://18.190.144.176";
}
    
builder.Services.AddClientServices(url);
builder.Services.AddMudServices();

await builder.Build().RunAsync();