namespace HsaLedger.Application.Requests;

public class SetPersonRequest
{
    public int PersonId { get; set; }
    public required string Name { get; set; }
}