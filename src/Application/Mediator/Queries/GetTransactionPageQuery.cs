using System.Diagnostics.CodeAnalysis;
using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Requests;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using HsaLedger.Application.Responses.Models;
using HsaLedger.Application.Responses.Pagination;

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
            if (string.IsNullOrWhiteSpace(filter.Value) && !filter.Operator.Contains("empty", StringComparison.InvariantCultureIgnoreCase))
                continue;

            var value = filter.Value;
            var op = filter.Operator.ToLowerInvariant();

            switch (filter.Property)
            {
                case "TransactionType":
                    switch (op)
                    {
                        case "contains":
                            query = query.Where(t => t.TransactionType.Code.Contains(value ?? ""));
                            break;
                        case "not contains":
                            query = query.Where(t => !t.TransactionType.Code.Contains(value ?? ""));
                            break;
                        case "equals":
                            query = query.Where(t => t.TransactionType.Code == value);
                            break;
                        case "not equals":
                            query = query.Where(t => t.TransactionType.Code != value);
                            break;
                        case "starts with":
                            query = query.Where(t => t.TransactionType.Code.StartsWith(value ?? ""));
                            break;
                        case "ends with":
                            query = query.Where(t => t.TransactionType.Code.EndsWith(value ?? ""));
                            break;
                        case "is empty":
                            query = query.Where(t => string.IsNullOrEmpty(t.TransactionType.Code));
                            break;
                        case "is not empty":
                            query = query.Where(t => !string.IsNullOrEmpty(t.TransactionType.Code));
                            break;
                    }
                    break;

                case "Amount":
                    if (decimal.TryParse(value, out var amount))
                    {
                        switch (op)
                        {
                            case "=":
                                query = query.Where(t => t.Amount / 100m == amount);
                                break;
                            case "!=":
                                query = query.Where(t => t.Amount / 100m != amount);
                                break;
                            case ">":
                                query = query.Where(t => t.Amount / 100m > amount);
                                break;
                            case ">=":
                                query = query.Where(t => t.Amount / 100m >= amount);
                                break;
                            case "<":
                                query = query.Where(t => t.Amount / 100m < amount);
                                break;
                            case "<=":
                                query = query.Where(t => t.Amount / 100m <= amount);
                                break;
                        }
                    }
                    else if (op == "is empty")
                    {
                        query = query.Where(t => t.Amount == 0);
                    }
                    else if (op == "is not empty")
                    {
                        query = query.Where(t => t.Amount != 0);
                    }
                    break;

                case "Date":
                    if (DateTime.TryParse(value, out var date))
                    {
                        switch (op)
                        {
                            case "is":
                                query = query.Where(t => t.Date.Date == date.Date);
                                break;
                            case "is not":
                                query = query.Where(t => t.Date.Date != date.Date);
                                break;
                            case "is after":
                                query = query.Where(t => t.Date > date);
                                break;
                            case "is on or after":
                                query = query.Where(t => t.Date >= date);
                                break;
                            case "is before":
                                query = query.Where(t => t.Date < date);
                                break;
                            case "is on or before":
                                query = query.Where(t => t.Date <= date);
                                break;
                        }
                    }
                    else if (op == "is empty")
                    {
                        // Assuming Date is nullable
                        query = query.Where(t => false);
                    }
                    else if (op == "is not empty")
                    {
                        query = query.Where(t => true);
                    }
                    break;

                case "Provider":
                    switch (op)
                    {
                        case "contains":
                            query = query.Where(t => t.Provider.Name.Contains(value ?? ""));
                            break;
                        case "not contains":
                            query = query.Where(t => !t.Provider.Name.Contains(value ?? ""));
                            break;
                        case "equals":
                            query = query.Where(t => t.Provider.Name == value);
                            break;
                        case "not equals":
                            query = query.Where(t => t.Provider.Name != value);
                            break;
                        case "starts with":
                            query = query.Where(t => t.Provider.Name.StartsWith(value ?? ""));
                            break;
                        case "ends with":
                            query = query.Where(t => t.Provider.Name.EndsWith(value ?? ""));
                            break;
                        case "is empty":
                            query = query.Where(t => string.IsNullOrEmpty(t.Provider.Name));
                            break;
                        case "is not empty":
                            query = query.Where(t => !string.IsNullOrEmpty(t.Provider.Name));
                            break;
                    }
                    break;

                case "Person":
                    switch (op)
                    {
                        case "contains":
                            query = query.Where(t => t.Person != null && t.Person.Name.Contains(value ?? ""));
                            break;
                        case "not contains":
                            query = query.Where(t => t.Person != null && !t.Person.Name.Contains(value ?? ""));
                            break;
                        case "equals":
                            query = query.Where(t => t.Person != null && t.Person.Name == value);
                            break;
                        case "not equals":
                            query = query.Where(t => t.Person != null && t.Person.Name != value);
                            break;
                        case "starts with":
                            query = query.Where(t => t.Person != null && t.Person.Name.StartsWith(value ?? ""));
                            break;
                        case "ends with":
                            query = query.Where(t => t.Person != null && t.Person.Name.EndsWith(value ?? ""));
                            break;
                        case "is empty":
                            query = query.Where(t => t.Person == null || string.IsNullOrEmpty(t.Person.Name));
                            break;
                        case "is not empty":
                            query = query.Where(t => t.Person != null && !string.IsNullOrEmpty(t.Person.Name));
                            break;
                    }
                    break;

                case "IsPaid":
                    if (bool.TryParse(value, out var isPaid))
                        query = query.Where(t => t.IsPaid == isPaid);
                    break;

                case "IsHsaWithdrawn":
                    if (bool.TryParse(value, out var isHsaWithdrawn))
                        query = query.Where(t => t.IsHsaWithdrawn == isHsaWithdrawn);
                    break;

                case "IsAudited":
                    if (bool.TryParse(value, out var isAudited))
                        query = query.Where(t => t.IsAudited == isAudited);
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
        else
        {
            // Default sort descending by Date
            query = query.OrderByDescending(x => x.Date);
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