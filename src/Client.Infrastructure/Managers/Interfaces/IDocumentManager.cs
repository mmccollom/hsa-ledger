using HsaLedger.Application.Requests;
using HsaLedger.Application.Responses.Projections;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Client.Infrastructure.Managers.Interfaces;

public interface IDocumentManager : IManager
{
    Task<IResult<DocumentResponse>> Get(int documentId);
    Task<IResult<int?>> Put(AddDocumentRequest documentRequest);
    Task<IResult<int?>> Post(SetDocumentRequest documentRequest);
    Task<IResult<int?>> Delete(int documentId);
}