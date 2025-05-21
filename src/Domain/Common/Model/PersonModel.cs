using System.Linq.Expressions;

namespace HsaLedger.Domain.Common.Model;

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
    
    public static Expression<Func<Entities.Person, PersonModel>> Projection
    {
        get
        {
            return x => new PersonModel
            {
                PersonId = x.PersonId,
                Name = x.Name,
                AllowDelete = x.Transactions.Count == 0,
                CreatedTime = x.CreatedTime,
                CreatedBy = x.CreatedBy,
                LastUpdatedTime = x.LastUpdatedTime,
                LastUpdatedBy = x.LastUpdatedBy,
                LockId = x.LockId,
            };
        }
    }
    
    public static PersonModel FromEntity(Entities.Person entity)
    {
        return Projection.Compile().Invoke(entity);
    }
}