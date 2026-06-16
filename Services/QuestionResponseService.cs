using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Services;

public class QuestionResponseService(PmsDbContext dbContext) : IQuestionResponseService
{
    public async Task<QuestionAnswerDto?> SubmitAsync(QuestionResponseCreateRequest request, CancellationToken cancellationToken = default)
    {
        var assignment = await dbContext.QuestionnaireAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(assignment => assignment.QuestionnaireAssignmentId == request.QuestionnaireAssignmentId, cancellationToken);

        if (assignment is null)
        {
            return null;
        }

        var question = await dbContext.Questions
            .AsNoTracking()
            .Include(question => question.Choices)
            .FirstOrDefaultAsync(question => question.QuestionId == request.QuestionId, cancellationToken);

        if (question is null)
        {
            return null;
        }

        if (request.SelectedQuestionChoiceIds.Count > 0)
        {
            var validChoiceIds = question.Choices.Select(choice => choice.SelectableQuestionChoiceId).ToHashSet();
            if (request.SelectedQuestionChoiceIds.Any(choiceId => !validChoiceIds.Contains(choiceId)))
            {
                return null;
            }
        }

        var response = new QuestionResponse
        {
            QuestionnaireAssignmentId = request.QuestionnaireAssignmentId,
            QuestionId = request.QuestionId,
            TextValue = request.TextValue,
            NumericValue = request.NumericValue,
            SelectedChoiceIds = request.SelectedQuestionChoiceIds.Count == 0 ? null : JsonSerializer.Serialize(request.SelectedQuestionChoiceIds),
            AnsweredDate = DateTime.UtcNow
        };

        dbContext.QuestionResponses.Add(response);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(response.QuestionResponseId, cancellationToken);
    }

    public async Task<IReadOnlyList<QuestionAnswerDto>> GetByAssignmentAsync(int questionnaireAssignmentId, CancellationToken cancellationToken = default)
    {
        var responses = await dbContext.QuestionResponses
            .AsNoTracking()
            .Where(response => response.QuestionnaireAssignmentId == questionnaireAssignmentId)
            .OrderBy(response => response.QuestionId)
            .ToListAsync(cancellationToken);

        return [.. responses.Select(DtoMapper.ToDto)];
    }

    private async Task<QuestionAnswerDto?> GetByIdAsync(int questionResponseId, CancellationToken cancellationToken)
    {
        var response = await dbContext.QuestionResponses
            .AsNoTracking()
            .FirstOrDefaultAsync(response => response.QuestionResponseId == questionResponseId, cancellationToken);

        return response is null ? null : DtoMapper.ToDto(response);
    }
}
