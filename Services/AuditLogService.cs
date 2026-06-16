using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Responses;

namespace PMS.Services;

public class AuditLogService(PmsDbContext dbContext) : IAuditLogService
{
    public async Task<IReadOnlyList<AuditLogDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var auditLogs = await dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(auditLog => auditLog.CreatedDate)
            .ToListAsync(cancellationToken);

        return auditLogs.Select(ToResponse).ToList();
    }

    public async Task WriteAsync(string entityName, int entityId, string action, string? details = null, CancellationToken cancellationToken = default)
    {
        dbContext.AuditLogs.Add(new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            Details = details,
            CreatedDate = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static AuditLogDto ToResponse(AuditLog auditLog)
    {
        return new AuditLogDto
        {
            AuditLogId = auditLog.AuditLogId,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            Action = auditLog.Action,
            Details = auditLog.Details,
            CreatedDate = auditLog.CreatedDate
        };
    }
}
