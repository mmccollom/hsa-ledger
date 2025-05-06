using HsaLedger.Application.Mediator.Commands;
using HsaLedger.Application.Mediator.Queries;
using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HsaLedger.Server.Controllers;

[Authorize]
public class DocumentController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<IEnumerable<DocumentResponse>>> Get()
    {
        var query = new GetDocumentQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpPut]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Add(AddDocumentRequest document)
    {
        var command = new AddDocumentCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpDelete]
    [Route("{documentId::int}"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Delete(int documentId)
    {
        var command = new DeleteDocumentCommand(documentId);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Set(SetDocumentRequest document)
    {
        var command = new SetDocumentCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
}