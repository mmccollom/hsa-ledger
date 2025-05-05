namespace HsaLedger.Application.Common.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime CentralStandardTimeNow { get; }
}