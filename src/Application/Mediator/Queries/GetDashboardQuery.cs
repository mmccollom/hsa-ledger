using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Application.Responses.SimpleDto;
using HsaLedger.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HsaLedger.Application.Mediator.Queries;

public class GetDashboardQuery : IRequest<Result<DashboardResponse>>
{
    
}

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardResponse>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var countOfTransactions = await _context.Transactions.CountAsync(cancellationToken: cancellationToken);
        var totalAmount = await _context.Transactions.SumAsync(x => x.Amount / 100.0m, cancellationToken: cancellationToken);
        var totalPaidAmount = await _context.Transactions.Where(x => x.IsPaid).SumAsync(x => x.Amount / 100.0m, cancellationToken: cancellationToken);
        var totalWithdrawnAmount = await _context.Transactions.Where(x => x.IsHsaWithdrawn).SumAsync(x => x.Amount / 100.0m, cancellationToken: cancellationToken);

        var dashboardResponse = new DashboardResponse
        {
            TotalSpent = totalAmount,
            Paid = totalPaidAmount,
            Unpaid = totalAmount - totalPaidAmount,
            Withdrawn = totalWithdrawnAmount,
            NotWithdrawn = totalAmount - totalWithdrawnAmount,
            CountOfTransactions = countOfTransactions,
        };

        return await Result<DashboardResponse>.SuccessAsync(dashboardResponse);
    }
}