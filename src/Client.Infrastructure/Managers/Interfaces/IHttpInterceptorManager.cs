using Toolbelt.Blazor;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IHttpInterceptorManager : IManager
{
    void RegisterEvent();

    Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

    void DisposeEvent();
}