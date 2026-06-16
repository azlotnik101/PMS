namespace PMS.Entities;

public class QuestionOption
{
    public int QuestionOptionId { get; set; }

    public int QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public Question Question { get; set; } = null!;
}
