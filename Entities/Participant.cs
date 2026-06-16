namespace PMS.Entities;

public class Participant
{
    public int ParticipantId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string ParticipantCode { get; set; } = null!;

    public DateTime StartDate { get; set; }
}
