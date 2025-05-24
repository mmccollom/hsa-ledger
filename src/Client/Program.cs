using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HsaLedger.Client;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var url = string.Empty;

if (builder.HostEnvironment.IsDevelopment())
{
    //url = "http://localhost:8081";
    url = "http://localhost:5000";
}
else
{
    // TODO: Add production url
    url = "https://api.hsaledger.com";
}
    
builder.Services.AddClientServices(url);
builder.Services.AddMudServices();

var sp = builder.Build();
var httpInterceptor = sp.Services.GetRequiredService<IHttpInterceptorManager>();
httpInterceptor.RegisterEvent(); // ✅ ensure this happens before anything uses it
    
await sp.RunAsync();