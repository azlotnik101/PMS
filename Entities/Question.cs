namespace PMS.Entities;

public class Question
{
    public int QuestionId { get; set; }

    public int QuestionnaireId { get; set; }

    public string Text { get; set; } = null!;

    public QuestionType QuestionType { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentQuestionId { get; set; }

    public int? ParentOptionId { get; set; }

    public Questionnaire Questionnaire { get; set; } = null!;

    public Question? ParentQuestion { get; set; }

    public QuestionOption? ParentOption { get; set; }

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
