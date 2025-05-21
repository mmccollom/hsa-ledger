using HsaLedger.Application.Mediator.Commands;
using HsaLedger.Application.Mediator.Queries;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Models;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

[Authorize]
public class TransactionTypeController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Operations,Service")]
    public async Task<Result<IEnumerable<TransactionTypeResponse>>> Get()
    {
        var query = new GetTransactionTypeQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet]
    [Route("getUiModel"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<IEnumerable<TransactionTypeModel>>> GetUiModel()
    {
        var query = new GetTransactionTypeModelQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpPut]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Add(AddTransactionTypeRequest document)
    {
        var command = new AddTransactionTypeCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpDelete]
    [Route("{transactionTypeId::int}"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Delete(int transactionTypeId)
    {
        var command = new DeleteTransactionTypeCommand(transactionTypeId);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Set(SetTransactionTypeRequest document)
    {
        var command = new SetTransactionTypeCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
}