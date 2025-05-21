using System.Diagnostics.CodeAnalysis;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using HsaLedger.Application.Responses.Pagination;
using HsaLedger.Domain.Common.Model;

namespace HsaLedger.Application.Mediator.Queries;

public class GetTransactionPageQuery : IRequest<Result<GridQueryResponse<TransactionModel>>>
{
    public GetTransactionPageQuery(GridQueryRequest gridQueryRequest)
    {
        GridQueryRequest = gridQueryRequest;
    }

    public GridQueryRequest GridQueryRequest { get; set; }
}

[SuppressMessage("ReSharper", "AccessToModifiedClosure")]
public class GetTransactionPageQueryHandler : IRequestHandler<GetTransactionPageQuery, Result<GridQueryResponse<TransactionModel>>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionPageQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GridQueryResponse<TransactionModel>>> Handle(GetTransactionPageQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .Include(x => x.Provider)
            .Include(x => x.Person)
            .Include(x => x.TransactionType)
            .Include(x => x.Documents)
            .AsQueryable();
        
        #region Filter
        foreach (var filter in request.GridQueryRequest.Filters ?? [])
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
                continue;

            switch (filter.Property)
            {
                case "TransactionType":
                    if (filter.Operator == "Equals")
                    {
                        query = query.Where(t => t.TransactionType.Code == filter.Value);
                    }
                    else if (filter.Operator == "Contains")
                    {
                        query = query.Where(t => t.TransactionType.Code.Contains(filter.Value));
                    }
                    break;

                case "Amount":
                    if (decimal.TryParse(filter.Value, out var amount))
                    {
                        switch (filter.Operator)
                        {
                            case "Equals":
                                query = query.Where(t => t.Amount / 100m == amount);
                                break;
                            case "GreaterThan":
                                query = query.Where(t => t.Amount / 100m > amount);
                                break;
                            case "LessThan":
                                query = query.Where(t => t.Amount / 100m < amount);
                                break;
                        }
                    }
                    break;

                case "Date":
                    if (DateTime.TryParse(filter.Value, out var date))
                    {
                        switch (filter.Operator)
                        {
                            case "Equals":
                                query = query.Where(t => t.Date == date.Date);
                                break;
                            case "Before":
                                query = query.Where(t => t.Date < date);
                                break;
                            case "After":
                                query = query.Where(t => t.Date > date);
                                break;
                        }
                    }
                    break;

                case "Provider":
                    if (filter.Operator == "Equals")
                    {
                        query = query.Where(x => x.Provider.Name == filter.Value);
                    }
                    else if (filter.Operator == "Contains")
                    {
                        query = query.Where(x => x.Provider.Name.Contains(filter.Value));
                    }
                    break;

                case "Person":
                    if (filter.Operator == "Equals")
                    {
                        query = query.Where(x => x.Person != null && x.Person.Name == filter.Value);
                    }
                    else if (filter.Operator == "Contains")
                    {
                        query = query.Where(x => x.Person != null && x.Person.Name.Contains(filter.Value));
                    }
                    break;

                case "IsPaid":
                    if (bool.TryParse(filter.Value, out var isPaid))
                    {
                        query = query.Where(t => t.IsPaid == isPaid);
                    }
                    break;

                case "IsHsaWithdrawn":
                    if (bool.TryParse(filter.Value, out var isHsaWithdrawn))
                    {
                        query = query.Where(t => t.IsHsaWithdrawn == isHsaWithdrawn);
                    }
                    break;

                case "IsAudited":
                    if (bool.TryParse(filter.Value, out var isAudited))
                    {
                        query = query.Where(t => t.IsAudited == isAudited);
                    }
                    break;
            }
        }

        #endregion

        #region Sort
        if (request.GridQueryRequest.Sorts is { Count: > 0 })
        {
            // Build sort expression string like: "Name ascending, Date descending"
            var sortExpressions = request.GridQueryRequest.Sorts
                .Select(s => $"{s.Property} {(s.Direction.ToLowerInvariant() == "descending" ? "descending" : "ascending")}")
                .ToList();

            // Apply using Dynamic LINQ (System.Linq.Dynamic.Core)
            query = query.OrderBy(string.Join(", ", sortExpressions));
        }

        #endregion

        var data = await query
            .Skip(request.GridQueryRequest.Page * request.GridQueryRequest.PageSize)
            .Take(request.GridQueryRequest.PageSize)
            .Select(TransactionModel.Projection)
            .ToListAsync(cancellationToken: cancellationToken);
            

        var totalCount = await query.CountAsync(cancellationToken: cancellationToken);

        var gridQueryResponse = new GridQueryResponse<TransactionModel>
        {
            TotalItems = totalCount,
            Items = data
        };
        
        return await Result<GridQueryResponse<TransactionModel>>.SuccessAsync(gridQueryResponse);
    }
}