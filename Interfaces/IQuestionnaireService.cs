using PMS.Models.Responses;

namespace PMS.Interfaces;

public interface IQuestionnaireService
{
    Task<IReadOnlyList<QuestionnaireDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<QuestionnaireDto?> GetByIdAsync(int questionnaireId, CancellationToken cancellationToken = default);
}