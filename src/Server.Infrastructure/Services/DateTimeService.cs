using HsaLedger.Application.Common.Interfaces;
using HsaLedger.Server.Infrastructure.Common.Extensions;

namespace HsaLedger.Server.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime CentralStandardTimeNow => DateTime.UtcNow.ConvertToTimeZoneFromUtc("America/Chicago");
}