using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IProviderManager : IManager
{
    Task<IResult<IEnumerable<ProviderResponse>>> Get();
    Task<IResult<IEnumerable<ProviderModel>>> GetUiModel();
    Task<IResult<int?>> Put(AddProviderRequest providerRequest);
    Task<IResult<int?>> Post(SetProviderRequest providerRequest);
    Task<IResult<int?>> Delete(int providerId);
}