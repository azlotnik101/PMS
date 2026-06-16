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
                    .ThenInclude(question => question.Choices)
            .Where(assignment => assignment.ParticipantId == participantId)
            .OrderByDescending(assignment => assignment.AssignedDate)
            .ToListAsync(cancellationToken);

        return [.. assignments.Select(DtoMapper.ToDto)];
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
                    .ThenInclude(question => question.Choices)
            .FirstOrDefaultAsync(assignment => assignment.QuestionnaireAssignmentId == questionnaireAssignmentId, cancellationToken);

        return assignment is null ? null : DtoMapper.ToDto(assignment);
    }
}
