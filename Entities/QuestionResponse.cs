namespace PMS.Entities;

public class QuestionResponse
{
    public int QuestionResponseId { get; set; }

    public int QuestionnaireAssignmentId { get; set; }

    public int QuestionId { get; set; }

    public string? TextValue { get; set; }

    public decimal? NumericValue { get; set; }

    public string? SelectedChoiceIds { get; set; }

    public DateTime AnsweredDate { get; set; }

    public QuestionnaireAssignment QuestionnaireAssignment { get; set; } = null!;

    public Question Question { get; set; } = null!;
}
