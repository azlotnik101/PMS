namespace PMS.Models.Responses;

public class QuestionnaireAssignmentDto
{
    public int QuestionnaireAssignmentId { get; set; }

    public int ParticipantId { get; set; }

    public int QuestionnaireId { get; set; }

    public DateTime AssignedDate { get; set; }

    public bool Completed { get; set; }

    public ParticipantDto? Participant { get; set; }

    public QuestionnaireDto? Questionnaire { get; set; }
}
