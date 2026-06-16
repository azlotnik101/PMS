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
            .Include(question => question.Options)
            .FirstOrDefaultAsync(question => question.QuestionId == request.QuestionId, cancellationToken);

        if (question is null)
        {
            return null;
        }

        if (request.SelectedQuestionOptionIds.Count > 0)
        {
            var validOptionIds = question.Options.Select(option => option.QuestionOptionId).ToHashSet();
            if (request.SelectedQuestionOptionIds.Any(optionId => !validOptionIds.Contains(optionId)))
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
            SelectedOptionIds = request.SelectedQuestionOptionIds.Count == 0 ? null : JsonSerializer.Serialize(request.SelectedQuestionOptionIds),
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

        return responses.Select(ToResponse).ToList();
    }

    private async Task<QuestionAnswerDto?> GetByIdAsync(int questionResponseId, CancellationToken cancellationToken)
    {
        var response = await dbContext.QuestionResponses
            .AsNoTracking()
            .FirstOrDefaultAsync(response => response.QuestionResponseId == questionResponseId, cancellationToken);

        return response is null ? null : ToResponse(response);
    }

    private static QuestionAnswerDto ToResponse(QuestionResponse response)
    {
        return new QuestionAnswerDto
        {
            QuestionResponseId = response.QuestionResponseId,
            QuestionnaireAssignmentId = response.QuestionnaireAssignmentId,
            QuestionId = response.QuestionId,
            TextValue = response.TextValue,
            NumericValue = response.NumericValue,
            AnsweredDate = response.AnsweredDate,
            SelectedQuestionOptionIds = ToSelectedQuestionOptionIds(response.SelectedOptionIds)
        };
    }

    private static List<int> ToSelectedQuestionOptionIds(string? selectedOptionIds)
    {
        if (string.IsNullOrWhiteSpace(selectedOptionIds))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<int>>(selectedOptionIds) ?? [];
    }
}
