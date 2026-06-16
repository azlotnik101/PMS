using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using PMS.Interfaces;
using PMS.Models.Requests;
using PMS.Models.Responses;

namespace PMS.Services;

public class ParticipantService(PmsDbContext dbContext, IAuditLogService auditLogService) : IParticipantService
{
    public async Task<IReadOnlyList<ParticipantDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var participants = await dbContext.Participants
            .AsNoTracking()
            .OrderBy(participant => participant.LastName)
            .ThenBy(participant => participant.FirstName)
            .ToListAsync(cancellationToken);

        return [.. participants.Select(ToResponse)];
    }

    public async Task<ParticipantDto?> GetByIdAsync(int participantId, CancellationToken cancellationToken = default)
    {
        var participant = await dbContext.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(participant => participant.ParticipantId == participantId, cancellationToken);

        return participant is null ? null : ToResponse(participant);
    }

    public async Task<ParticipantDto> CreateAsync(ParticipantCreateRequest request, CancellationToken cancellationToken = default)
    {
        var participant = new Participant();
        ApplyRequest(participant, request);

        dbContext.Participants.Add(participant);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(Participant), participant.ParticipantId, "Created", cancellationToken: cancellationToken);

        return ToResponse(participant);
    }

    public async Task<ParticipantDto?> UpdateAsync(int participantId, ParticipantUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var participant = await dbContext.Participants.FindAsync([participantId], cancellationToken);
        if (participant is null)
        {
            return null;
        }

        ApplyRequest(participant, request);

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(nameof(Participant), participant.ParticipantId, "Updated", cancellationToken: cancellationToken);

        return ToResponse(participant);
    }

    public async Task<bool> DeleteAsync(int participantId, CancellationToken cancellationToken = default)
    {
        var participant = await dbContext.Participants.FindAsync([participantId], cancellationToken);
        if (participant is null)
        {
            return false;
        }

        dbContext.Participants.Remove(participant);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ApplyRequest(Participant participant, ParticipantCreateRequest request)
    {
        participant.FirstName = request.FirstName;
        participant.LastName = request.LastName;
        participant.EmailAddress = request.EmailAddress;
        participant.DateOfBirth = request.DateOfBirth;
        participant.ParticipantCode = request.ParticipantCode;
        participant.StartDate = request.StartDate;
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
}
