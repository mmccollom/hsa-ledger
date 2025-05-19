using HsaLedger.Application.Mediator.Commands;
using HsaLedger.Application.Mediator.Queries;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

[Authorize]
public class TransactionController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<IEnumerable<TransactionResponse>>> Get()
    {
        var query = new GetTransactionQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet]
    [Route("documents"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<IEnumerable<DocumentResponse>>> GetDocuments(int transactionId)
    {
        var query = new GetDocumentByTransactionIdQuery(transactionId);
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpPut]
    [Authorize(Roles = "Administrator,Operations,Service")]
    public async Task<Result<int>> Add(AddTransactionRequest document)
    {
        var command = new AddTransactionCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpDelete]
    [Route("{transactionId::int}"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Delete(int transactionId)
    {
        var command = new DeleteTransactionCommand(transactionId);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Set(SetTransactionRequest document)
    {
        var command = new SetTransactionCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
}