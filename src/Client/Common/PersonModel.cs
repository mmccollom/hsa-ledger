using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common;

public class PersonModel
{
    public int PersonId { get; set; }
    public required string Name { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static PersonModel FromPersonResponse(PersonResponse response)
    {
        return new PersonModel
        {
            PersonId = response.PersonId,
            Name = response.Name,
            AllowDelete = response.AllowDelete,
            CreatedTime = response.CreatedTime,
            CreatedBy = response.CreatedBy,
            LastUpdatedTime = response.LastUpdatedTime,
            LastUpdatedBy = response.LastUpdatedBy,
            LockId = response.LockId,
        };
    }
}