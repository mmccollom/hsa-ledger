using HsaLedger.Application.Mediator.Commands;
using HsaLedger.Application.Mediator.Queries;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Pagination;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Application.Responses.SimpleDto;
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
    
    [HttpPost]
    [Route("paged"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<GridQueryResponse<TransactionResponse>>> GetPaged(GridQueryRequest gridQueryRequest)
    {
        var query = new GetTransactionPageQuery(gridQueryRequest);
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet]
    [Route("dashboard"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<DashboardResponse>> GetDashboard()
    {
        var query = new GetDashboardQuery();
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
    public async Task<Result<int>> Add(AddTransactionRequest transaction)
    {
        var command = new AddTransactionCommand(transaction);
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
    public async Task<Result<int>> Set(SetTransactionRequest transaction)
    {
        var command = new SetTransactionCommand(transaction);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpPost]
    [Route("MassUpdate"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> SetInMass(List<SetTransactionRequest> transactions)
    {
        var command = new SetTransactionMassCommand(transactions);
        var result = await Mediator.Send(command);
        return result;
    }
}