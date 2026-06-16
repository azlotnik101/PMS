using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Services;

public class QuestionnaireAssignmentService(PmsDbContext dbContext, IAuditLogService auditLogService) : IQuestionnaireAssignmentService
{
    public async Task<QuestionnaireAssignmentDto?> AssignAsync(QuestionnaireAssignmentCreateRequest request, CancellationToken cancellationToken = default)
    {
        var participantExists = await dbContext.Participants.AnyAsync(participant => participant.ParticipantId == request.ParticipantId, cancellationToken);
        var questionnaireExists = await dbContext.Questionnaires.AnyAsync(questionnaire => questionnaire.QuestionnaireId == request.QuestionnaireId, cancellationToken);

        if (!participantExists || !questionnaireExists)
        {
            return null;
        }

        var assignment = new QuestionnaireAssignment
        {
            ParticipantId = request.ParticipantId,
            QuestionnaireId = request.QuestionnaireId,
            AssignedDate = request.AssignedDate ?? DateTime.UtcNow,
            Completed = false
        };

        dbContext.QuestionnaireAssignments.Add(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(QuestionnaireAssignment), assignment.QuestionnaireAssignmentId, "Created", cancellationToken: cancellationToken);

        return await GetByIdAsync(assignment.QuestionnaireAssignmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<QuestionnaireAssignmentDto>> GetByParticipantAsync(int participantId, CancellationToken cancellationToken = default)
    {
        var assignments = await dbContext.QuestionnaireAssignments
            .AsNoTracking()
            .Include(assignment => assignment.Participant)
            .Include(assignment => assignment.Questionnaire)
                .ThenInclude(questionnaire => questionnaire.Questions)
                    .ThenInclude(question => question.Options)
            .Where(assignment => assignment.ParticipantId == participantId)
            .OrderByDescending(assignment => assignment.AssignedDate)
            .ToListAsync(cancellationToken);

        return assignments.Select(ToResponse).ToList();
    }

    public async Task<QuestionnaireAssignmentDto?> CompleteAsync(int questionnaireAssignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await dbContext.QuestionnaireAssignments.FindAsync([questionnaireAssignmentId], cancellationToken);
        if (assignment is null)
        {
            return null;
        }

        assignment.Completed = true;

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(QuestionnaireAssignment), assignment.QuestionnaireAssignmentId, "Completed", cancellationToken: cancellationToken);

        return await GetByIdAsync(assignment.QuestionnaireAssignmentId, cancellationToken);
    }

    private async Task<QuestionnaireAssignmentDto?> GetByIdAsync(int questionnaireAssignmentId, CancellationToken cancellationToken)
    {
        var assignment = await dbContext.QuestionnaireAssignments
            .AsNoTracking()
            .Include(assignment => assignment.Participant)
            .Include(assignment => assignment.Questionnaire)
                .ThenInclude(questionnaire => questionnaire.Questions)
                    .ThenInclude(question => question.Options)
            .FirstOrDefaultAsync(assignment => assignment.QuestionnaireAssignmentId == questionnaireAssignmentId, cancellationToken);

        return assignment is null ? null : ToResponse(assignment);
    }

    private static QuestionnaireAssignmentDto ToResponse(QuestionnaireAssignment assignment)
    {
        return new QuestionnaireAssignmentDto
        {
            QuestionnaireAssignmentId = assignment.QuestionnaireAssignmentId,
            ParticipantId = assignment.ParticipantId,
            QuestionnaireId = assignment.QuestionnaireId,
            AssignedDate = assignment.AssignedDate,
            Completed = assignment.Completed,
            Participant = assignment.Participant is null ? null : ToResponse(assignment.Participant),
            Questionnaire = assignment.Questionnaire is null ? null : ToResponse(assignment.Questionnaire)
        };
    }

    private static ParticipantDto ToResponse(Participant participant)
    {
        return new ParticipantDto
        {
            ParticipantId = participant.ParticipantId,
            FirstName = participant.FirstName,
            LastName = participant.LastName,
            EmailAddress = participant.EmailAddress,
            DateOfBirth = participant.DateOfBirth,
            ParticipantCode = participant.ParticipantCode,
            StartDate = participant.StartDate
        };
    }

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
}
