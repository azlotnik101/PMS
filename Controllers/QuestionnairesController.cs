using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionnairesController(IQuestionnaireService questionnaireService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<QuestionnaireDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await questionnaireService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{questionnaireId:int}")]
    public async Task<ActionResult<QuestionnaireDto>> GetById(int questionnaireId, CancellationToken cancellationToken)
    {
        var questionnaire = await questionnaireService.GetByIdAsync(questionnaireId, cancellationToken);
        return questionnaire is null ? NotFound() : Ok(questionnaire);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionnaireDto>> Create(QuestionnaireCreateRequest request, CancellationToken cancellationToken)
    {
        var questionnaire = await questionnaireService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { questionnaireId = questionnaire.QuestionnaireId }, questionnaire);
    }

    [HttpPut("{questionnaireId:int}")]
    public async Task<ActionResult<QuestionnaireDto>> Update(int questionnaireId, QuestionnaireUpdateRequest request, CancellationToken cancellationToken)
    {
        var questionnaire = await questionnaireService.UpdateAsync(questionnaireId, request, cancellationToken);
        return questionnaire is null ? NotFound() : Ok(questionnaire);
    }

    [HttpDelete("{questionnaireId:int}")]
    public async Task<IActionResult> Delete(int questionnaireId, CancellationToken cancellationToken)
    {
        return await questionnaireService.DeleteAsync(questionnaireId, cancellationToken) ? NoContent() : NotFound();
    }
}
