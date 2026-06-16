namespace PMS.Entities;

public class SelectableQuestionChoice
{
    public int SelectableQuestionChoiceId { get; set; }

    public int QuestionId { get; set; }

    public string ChoiceText { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public Question Question { get; set; } = null!;
}
