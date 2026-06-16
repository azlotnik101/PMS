using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Interfaces;

public interface IQuestionnaireAssignmentService
{
    Task<QuestionnaireAssignmentDto?> AssignAsync(QuestionnaireAssignmentCreateRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<QuestionnaireAssignmentDto>> GetByParticipantAsync(int participantId, CancellationToken cancellationToken = default);

    Task<QuestionnaireAssignmentDto?> CompleteAsync(int questionnaireAssignmentId, CancellationToken cancellationToken = default);
}
