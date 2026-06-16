using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Controllers;

[ApiController]
[Route("api/questionnaire-assignments")]
public class QuestionnaireAssignmentsController(IQuestionnaireAssignmentService questionnaireAssignmentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuestionnaireAssignmentDto>> Assign(QuestionnaireAssignmentCreateRequest request, CancellationToken cancellationToken)
    {
        var assignment = await questionnaireAssignmentService.AssignAsync(request, cancellationToken);
        return assignment is null ? BadRequest("Participant or questionnaire was not found.") : Ok(assignment);
    }

    [HttpGet("participant/{participantId:int}")]
    public async Task<ActionResult<IReadOnlyList<QuestionnaireAssignmentDto>>> GetByParticipant(int participantId, CancellationToken cancellationToken)
    {
        return Ok(await questionnaireAssignmentService.GetByParticipantAsync(participantId, cancellationToken));
    }

    [HttpPut("{questionnaireAssignmentId:int}/complete")]
    public async Task<ActionResult<QuestionnaireAssignmentDto>> Complete(int questionnaireAssignmentId, CancellationToken cancellationToken)
    {
        var assignment = await questionnaireAssignmentService.CompleteAsync(questionnaireAssignmentId, cancellationToken);
        return assignment is null ? NotFound() : Ok(assignment);
    }
}
