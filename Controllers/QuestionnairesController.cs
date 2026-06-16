using Microsoft.AspNetCore.Mvc;
using PMS.Interfaces;
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
}
