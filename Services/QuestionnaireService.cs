using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Services;

public class QuestionnaireService(PmsDbContext dbContext, IAuditLogService auditLogService) : IQuestionnaireService
{
    public async Task<IReadOnlyList<QuestionnaireDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var questionnaires = await dbContext.Questionnaires
            .AsNoTracking()
            .Include(questionnaire => questionnaire.Questions)
                .ThenInclude(question => question.Options)
            .OrderBy(questionnaire => questionnaire.Title)
            .ToListAsync(cancellationToken);

        return questionnaires.Select(ToResponse).ToList();
    }

    public async Task<QuestionnaireDto?> GetByIdAsync(int questionnaireId, CancellationToken cancellationToken = default)
    {
        var questionnaire = await GetQuestionnaireAsync(questionnaireId, true, cancellationToken);
        return questionnaire is null ? null : ToResponse(questionnaire);
    }

    public async Task<QuestionnaireDto> CreateAsync(QuestionnaireCreateRequest request, CancellationToken cancellationToken = default)
    {
        var questionnaire = new Questionnaire
        {
            Title = request.Title,
            Questions = request.Questions.Select(CreateQuestion).ToList()
        };

        dbContext.Questionnaires.Add(questionnaire);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(Questionnaire), questionnaire.QuestionnaireId, "Created", cancellationToken: cancellationToken);

        return ToResponse(questionnaire);
    }

    public async Task<QuestionnaireDto?> UpdateAsync(int questionnaireId, QuestionnaireUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var questionnaire = await GetQuestionnaireAsync(questionnaireId, false, cancellationToken);
        if (questionnaire is null)
        {
            return null;
        }

        questionnaire.Title = request.Title;
        SyncQuestions(questionnaire, request.Questions);

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(Questionnaire), questionnaire.QuestionnaireId, "Updated", cancellationToken: cancellationToken);

        return ToResponse(questionnaire);
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

    private async Task<Questionnaire?> GetQuestionnaireAsync(int questionnaireId, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = dbContext.Questionnaires
            .Include(questionnaire => questionnaire.Questions)
                .ThenInclude(question => question.Options)
            .Where(questionnaire => questionnaire.QuestionnaireId == questionnaireId);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    private static Question CreateQuestion(QuestionCreateRequest request) => new()
    {
        Text = request.Text,
        QuestionType = request.QuestionType,
        DisplayOrder = request.DisplayOrder,
        ParentQuestionId = request.ParentQuestionId,
        ParentOptionId = request.ParentOptionId,
        Options = request.Options.Select(CreateOption).ToList()
    };

    private static QuestionOption CreateOption(QuestionOptionCreateRequest request) => new()
    {
        OptionText = request.OptionText,
        DisplayOrder = request.DisplayOrder
    };

    private static QuestionnaireDto ToResponse(Questionnaire questionnaire)
    {
        return new QuestionnaireDto
        {
            QuestionnaireId = questionnaire.QuestionnaireId,
            Title = questionnaire.Title,
            Questions = questionnaire.Questions
                .OrderBy(question => question.DisplayOrder)
                .Select(ToResponse)
                .ToList()
        };
    }

    private static QuestionDto ToResponse(Question question)
    {
        return new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionnaireId = question.QuestionnaireId,
            Text = question.Text,
            QuestionType = question.QuestionType,
            DisplayOrder = question.DisplayOrder,
            ParentQuestionId = question.ParentQuestionId,
            ParentOptionId = question.ParentOptionId,
            Options = question.Options
                .OrderBy(option => option.DisplayOrder)
                .Select(ToResponse)
                .ToList()
        };
    }

    private static QuestionOptionDto ToResponse(QuestionOption option)
    {
        return new QuestionOptionDto
        {
            QuestionOptionId = option.QuestionOptionId,
            QuestionId = option.QuestionId,
            OptionText = option.OptionText,
            DisplayOrder = option.DisplayOrder
        };
    }

    private void SyncQuestions(Questionnaire questionnaire, List<QuestionUpdateRequest> requests)
    {
        var requestedQuestionIds = requests
            .Where(question => question.QuestionId.HasValue)
            .Select(question => question.QuestionId!.Value)
            .ToHashSet();

        var questionsToRemove = questionnaire.Questions
            .Where(question => !requestedQuestionIds.Contains(question.QuestionId))
            .ToList();

        dbContext.Questions.RemoveRange(questionsToRemove);

        foreach (var request in requests)
        {
            var question = request.QuestionId.HasValue
                ? questionnaire.Questions.FirstOrDefault(existingQuestion => existingQuestion.QuestionId == request.QuestionId.Value)
                : null;

            if (question is null)
            {
                questionnaire.Questions.Add(CreateQuestion(request));
                continue;
            }

            question.Text = request.Text;
            question.QuestionType = request.QuestionType;
            question.DisplayOrder = request.DisplayOrder;
            question.ParentQuestionId = request.ParentQuestionId;
            question.ParentOptionId = request.ParentOptionId;
            SyncOptions(question, request.Options);
        }
    }

    private void SyncOptions(Question question, List<QuestionOptionUpdateRequest> requests)
    {
        var requestedOptionIds = requests
            .Where(option => option.QuestionOptionId.HasValue)
            .Select(option => option.QuestionOptionId!.Value)
            .ToHashSet();

        var optionsToRemove = question.Options
            .Where(option => !requestedOptionIds.Contains(option.QuestionOptionId))
            .ToList();

        dbContext.QuestionOptions.RemoveRange(optionsToRemove);

        foreach (var request in requests)
        {
            var option = request.QuestionOptionId.HasValue
                ? question.Options.FirstOrDefault(existingOption => existingOption.QuestionOptionId == request.QuestionOptionId.Value)
                : null;

            if (option is null)
            {
                question.Options.Add(CreateOption(request));
                continue;
            }

            option.OptionText = request.OptionText;
            option.DisplayOrder = request.DisplayOrder;
        }
    }
}
