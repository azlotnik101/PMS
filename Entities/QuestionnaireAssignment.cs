namespace PMS.Entities;

public class QuestionnaireAssignment
{
    public int QuestionnaireAssignmentId { get; set; }

    public int ParticipantId { get; set; }

    public int QuestionnaireId { get; set; }

    public DateTime AssignedDate { get; set; }

    public bool Completed { get; set; }

    public Participant Participant { get; set; } = null!;

    public Questionnaire Questionnaire { get; set; } = null!;
}
