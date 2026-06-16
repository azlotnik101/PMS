using PMS.Models.Responses;

namespace PMS.Interfaces;

public interface IAuditLogService
{
    Task<IReadOnlyList<AuditLogDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task WriteAsync(string entityName, int entityId, string action, string? details = null, CancellationToken cancellationToken = default);
}