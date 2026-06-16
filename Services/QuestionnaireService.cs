using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Responses;

namespace PMS.Services;

public class QuestionnaireService(
    PmsDbContext dbContext) : IQuestionnaireService
{
    public async Task<IReadOnlyList<QuestionnaireDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var questionnaires = await GetQuestionnairesQuery()
            .AsNoTracking()
            .OrderBy(questionnaire => questionnaire.Title)
            .ToListAsync(cancellationToken);

        return [.. questionnaires.Select(DtoMapper.ToDto)];
    }

    public async Task<QuestionnaireDto?> GetByIdAsync(int questionnaireId, CancellationToken cancellationToken = default)
    {
        var questionnaire = await GetQuestionnaireAsync(questionnaireId, true, cancellationToken);
        return questionnaire is null ? null : DtoMapper.ToDto(questionnaire);
    }

    private IQueryable<Questionnaire> GetQuestionnairesQuery()
    {
        return dbContext.Questionnaires
            .Include(questionnaire => questionnaire.Questions)
                .ThenInclude(question => question.Choices);
    }

    private async Task<Questionnaire?> GetQuestionnaireAsync(int questionnaireId, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = GetQuestionnairesQuery()
            .Where(questionnaire => questionnaire.QuestionnaireId == questionnaireId);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}
