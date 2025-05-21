using HsaLedger.Application.Mediator.Commands;
using HsaLedger.Application.Mediator.Queries;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Domain.Common.Model;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

[Authorize]
public class ProviderController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Operations,Service")]
    public async Task<Result<IEnumerable<ProviderResponse>>> Get()
    {
        var query = new GetProviderQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet]
    [Route("getUiModel"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<IEnumerable<ProviderModel>>> GetUiModel()
    {
        var query = new GetProviderModelQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpPut]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Add(AddProviderRequest document)
    {
        var command = new AddProviderCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpDelete]
    [Route("{providerId::int}"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Delete(int providerId)
    {
        var command = new DeleteProviderCommand(providerId);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Set(SetProviderRequest document)
    {
        var command = new SetProviderCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
}