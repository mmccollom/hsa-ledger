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
public class PersonController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Operations,Service")]
    public async Task<Result<IEnumerable<PersonResponse>>> Get()
    {
        var query = new GetPersonQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet]
    [Route("getUiModel"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<IEnumerable<PersonModel>>> GetUiModel()
    {
        var query = new GetPersonModelQuery();
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpPut]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Add(AddPersonRequest document)
    {
        var command = new AddPersonCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpDelete]
    [Route("{personId::int}"), Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Delete(int personId)
    {
        var command = new DeletePersonCommand(personId);
        var result = await Mediator.Send(command);
        return result;
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator,Operations")]
    public async Task<Result<int>> Set(SetPersonRequest document)
    {
        var command = new SetPersonCommand(document);
        var result = await Mediator.Send(command);
        return result;
    }
}