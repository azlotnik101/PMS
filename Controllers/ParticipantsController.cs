using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController(IParticipantService participantService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ParticipantDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await participantService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{participantId:int}")]
    public async Task<ActionResult<ParticipantDto>> GetById(int participantId, CancellationToken cancellationToken)
    {
        var participant = await participantService.GetByIdAsync(participantId, cancellationToken);
        return participant is null ? NotFound() : Ok(participant);
    }

    [HttpPost]
    public async Task<ActionResult<ParticipantDto>> Create(ParticipantCreateRequest request, CancellationToken cancellationToken)
    {
        var participant = await participantService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { participantId = participant.ParticipantId }, participant);
    }

    [HttpPut("{participantId:int}")]
    public async Task<ActionResult<ParticipantDto>> Update(int participantId, ParticipantUpdateRequest request, CancellationToken cancellationToken)
    {
        var participant = await participantService.UpdateAsync(participantId, request, cancellationToken);
        return participant is null ? NotFound() : Ok(participant);
    }

    [HttpDelete("{participantId:int}")]
    public async Task<IActionResult> Delete(int participantId, CancellationToken cancellationToken)
    {
        return await participantService.DeleteAsync(participantId, cancellationToken) ? NoContent() : NotFound();
    }
}
