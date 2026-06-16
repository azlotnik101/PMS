using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Interfaces;

public interface IQuestionnaireService
{
    Task<IReadOnlyList<QuestionnaireDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<QuestionnaireDto?> GetByIdAsync(int questionnaireId, CancellationToken cancellationToken = default);

    Task<QuestionnaireDto> CreateAsync(QuestionnaireCreateRequest request, CancellationToken cancellationToken = default);

    Task<QuestionnaireDto?> UpdateAsync(int questionnaireId, QuestionnaireUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int questionnaireId, CancellationToken cancellationToken = default);
}
