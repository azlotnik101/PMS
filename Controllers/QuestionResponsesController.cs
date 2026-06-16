using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Controllers;

[ApiController]
[Route("api/question-responses")]
public class QuestionResponsesController(IQuestionResponseService questionResponseService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuestionAnswerDto>> Submit(QuestionResponseCreateRequest request, CancellationToken cancellationToken)
    {
        var response = await questionResponseService.SubmitAsync(request, cancellationToken);
        return response is null ? BadRequest("Assignment, question, or selected option was not found.") : Ok(response);
    }

    [HttpGet("assignment/{questionnaireAssignmentId:int}")]
    public async Task<ActionResult<IReadOnlyList<QuestionAnswerDto>>> GetByAssignment(int questionnaireAssignmentId, CancellationToken cancellationToken)
    {
        return Ok(await questionResponseService.GetByAssignmentAsync(questionnaireAssignmentId, cancellationToken));
    }
}
