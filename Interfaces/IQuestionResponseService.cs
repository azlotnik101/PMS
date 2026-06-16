using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Interfaces;

public interface IQuestionResponseService
{
    Task<QuestionAnswerDto?> SubmitAsync(QuestionResponseCreateRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuestionAnswerDto>> GetByAssignmentAsync(int questionnaireAssignmentId, CancellationToken cancellationToken = default);
}