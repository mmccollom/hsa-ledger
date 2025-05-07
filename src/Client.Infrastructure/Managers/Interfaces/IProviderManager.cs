using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IProviderManager : IManager
{
    Task<IResult<IEnumerable<ProviderResponse>>> Get();
    Task<IResult<int?>> Put(AddProviderRequest providerRequest);
    Task<IResult<int?>> Post(SetProviderRequest providerRequest);
    Task<IResult<int?>> Delete(int providerId);
}