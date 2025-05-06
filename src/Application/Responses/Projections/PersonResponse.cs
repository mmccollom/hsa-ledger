using System.Linq.Expressions;

namespace HsaLedger.Application.Responses.Projections;

public class PersonResponse
{
    public int PersonId { get; set; }
    public required string Name { get; set; }
    public bool AllowDelete { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public string? LastUpdatedBy { get; set; }
    public int LockId { get; set; }
    
    public static Expression<Func<Domain.Entities.Person, PersonResponse>> Projection
    {
        get
        {
            return x => new PersonResponse
            {
                PersonId = x.PersonId,
                Name = x.Name,
                AllowDelete = x.Transactions.Count == 0,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId
            };
        }
    }
    
    public static PersonResponse FromEntity(Domain.Entities.Person entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}