namespace PMS.Entities;

public class Questionnaire
{
    public int QuestionnaireId { get; set; }

    public string Title { get; set; } = null!;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
