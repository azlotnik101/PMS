namespace PMS.Entities;

public class AuditLog
{
    public long AuditLogId { get; set; }

    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? Details { get; set; }

    public DateTime CreatedDate { get; set; }
}
