namespace HsaLedger.Application.Requests;

public class SetDocumentRequest
{
    public int DocumentId { get; set; }
    public int TransactionId { get; set; }
    public required string Fullname { get; set; }
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required byte[] Content { get; set; }
}