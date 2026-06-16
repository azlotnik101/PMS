namespace PMS.Entities;

public class Question
{
    public int QuestionId { get; set; }

    public int QuestionnaireId { get; set; }

    public string Text { get; set; } = null!;

    public QuestionType QuestionType { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentQuestionId { get; set; }

    public int? ParentChoiceId { get; set; }

    public Questionnaire Questionnaire { get; set; } = null!;

    public Question? ParentQuestion { get; set; }

    public SelectableQuestionChoice? ParentChoice { get; set; }

    public ICollection<SelectableQuestionChoice> Choices { get; set; } = new List<SelectableQuestionChoice>();
}
