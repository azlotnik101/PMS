using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models.Responses;

namespace PMS.Controllers;

[ApiController]
[Route("api/audit-logs")]
public class AuditLogsController(IAuditLogService auditLogService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await auditLogService.GetAllAsync(cancellationToken));
    }
}
