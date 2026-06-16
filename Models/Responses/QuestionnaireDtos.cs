using PMS.Entities;

namespace PMS.Models.Responses;

public class QuestionnaireDto
{
    public int QuestionnaireId { get; set; }

    public string Title { get; set; } = null!;

    public List<QuestionDto> Questions { get; set; } = [];
}

public class QuestionDto
{
    public int QuestionId { get; set; }

    public int QuestionnaireId { get; set; }

    public string Text { get; set; } = null!;

    public QuestionType QuestionType { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentQuestionId { get; set; }

    public int? ParentOptionId { get; set; }

    public List<QuestionOptionDto> Options { get; set; } = [];
}

public class QuestionOptionDto
{
    public int QuestionOptionId { get; set; }

    public int QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public int DisplayOrder { get; set; }
}
