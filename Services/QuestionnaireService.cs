using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Services;

public class QuestionnaireService(
    PmsDbContext dbContext,
    IAuditLogService auditLogService,
    QuestionnaireRequestApplier requestApplier,
    QuestionnaireSynchronizer synchronizer,
    QuestionnaireResponseFactory responseFactory) : IQuestionnaireService
{
    public async Task<IReadOnlyList<QuestionnaireDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var questionnaires = await GetQuestionnairesQuery()
            .AsNoTracking()
            .OrderBy(questionnaire => questionnaire.Title)
            .ToListAsync(cancellationToken);

        return [.. questionnaires.Select(responseFactory.ToResponse)];
    }

    public async Task<QuestionnaireDto?> GetByIdAsync(int questionnaireId, CancellationToken cancellationToken = default)
    {
        var questionnaire = await GetQuestionnaireAsync(questionnaireId, true, cancellationToken);
        return questionnaire is null ? null : responseFactory.ToResponse(questionnaire);
    }

    public async Task<QuestionnaireDto> CreateAsync(QuestionnaireCreateRequest request, CancellationToken cancellationToken = default)
    {
        var questionnaire = requestApplier.CreateQuestionnaire(request);

        dbContext.Questionnaires.Add(questionnaire);
        await SaveAndAuditAsync(questionnaire, "Created", cancellationToken);

        return responseFactory.ToResponse(questionnaire);
    }

    public async Task<QuestionnaireDto?> UpdateAsync(int questionnaireId, QuestionnaireUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var questionnaire = await GetQuestionnaireAsync(questionnaireId, false, cancellationToken);
        if (questionnaire is null)
        {
            return null;
        }

        requestApplier.Apply(questionnaire, request);
        synchronizer.SyncQuestions(questionnaire, request.Questions);

        await SaveAndAuditAsync(questionnaire, "Updated", cancellationToken);

        return responseFactory.ToResponse(questionnaire);
    }

    public async Task<bool> DeleteAsync(int questionnaireId, CancellationToken cancellationToken = default)
    {
        var questionnaire = await dbContext.Questionnaires.FindAsync([questionnaireId], cancellationToken);
        if (questionnaire is null)
        {
            return false;
        }

        dbContext.Questionnaires.Remove(questionnaire);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
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

    private async Task SaveAndAuditAsync(Questionnaire questionnaire, string action, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(Questionnaire), questionnaire.QuestionnaireId, action, cancellationToken: cancellationToken);
    }
}
