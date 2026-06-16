namespace PMS.Models.Responses;

public class QuestionAnswerDto
{
    public int QuestionResponseId { get; set; }

    public int QuestionnaireAssignmentId { get; set; }

    public int QuestionId { get; set; }

    public string? TextValue { get; set; }

    public decimal? NumericValue { get; set; }

    public DateTime AnsweredDate { get; set; }

    public List<int> SelectedQuestionChoiceIds { get; set; } = [];
}
