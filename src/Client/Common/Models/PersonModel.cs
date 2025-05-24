using System.Linq.Expressions;
using HsaLedger.Application.Responses.Projections;

namespace HsaLedger.Client.Common.Models;

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
    
    public static Expression<Func<PersonResponse, PersonModel>> Projection
    {
        get
        {
            return x => new PersonModel
            {
                PersonId = x.PersonId,
                Name = x.Name,
                AllowDelete = x.AllowDelete,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId,
            };
        }
    }
    
    public static PersonModel FromResponse(PersonResponse response)
    {
        return Projection.Compile().Invoke(response);
    }
}