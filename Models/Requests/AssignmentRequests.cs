using System.ComponentModel.DataAnnotations;

namespace PMS.Models.Requests;

public class QuestionnaireAssignmentCreateRequest
{
    [Range(1, int.MaxValue)]
    public int ParticipantId { get; set; }

    [Range(1, int.MaxValue)]
    public int QuestionnaireId { get; set; }

    public DateTime? AssignedDate { get; set; }
}
