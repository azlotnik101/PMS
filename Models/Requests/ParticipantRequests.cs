using System.ComponentModel.DataAnnotations;

namespace PMS.Models.Requests;

public class ParticipantCreateRequest
{
    [Required]
    [MaxLength(200)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string EmailAddress { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(100)]
    public string ParticipantCode { get; set; } = null!;

    public DateTime StartDate { get; set; }
}

public class ParticipantUpdateRequest : ParticipantCreateRequest;
