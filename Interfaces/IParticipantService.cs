using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Interfaces;

public interface IParticipantService
{
    Task<IReadOnlyList<ParticipantDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ParticipantDto?> GetByIdAsync(int participantId, CancellationToken cancellationToken = default);

    Task<ParticipantDto> CreateAsync(ParticipantCreateRequest request, CancellationToken cancellationToken = default);

    Task<ParticipantDto?> UpdateAsync(int participantId, ParticipantUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int participantId, CancellationToken cancellationToken = default);
}
