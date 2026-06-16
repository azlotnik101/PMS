using System.ComponentModel.DataAnnotations;
using PMS.Entities;

namespace PMS.Models.Requests;

public class QuestionnaireCreateRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MinLength(1)]
    public List<QuestionCreateRequest> Questions { get; set; } = [];
}

public class QuestionnaireUpdateRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public List<QuestionUpdateRequest> Questions { get; set; } = [];
}

public class QuestionCreateRequest
{
    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = null!;

    public QuestionType QuestionType { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentQuestionId { get; set; }

    public int? ParentChoiceId { get; set; }

    public List<SelectableQuestionChoiceCreateRequest> Choices { get; set; } = [];
}

public class QuestionUpdateRequest : QuestionCreateRequest
{
    public int? QuestionId { get; set; }

    public new List<SelectableQuestionChoiceUpdateRequest> Choices { get; set; } = [];
}

public class SelectableQuestionChoiceCreateRequest
{
    [Required]
    [MaxLength(500)]
    public string ChoiceText { get; set; } = null!;

    public int DisplayOrder { get; set; }
}

public class SelectableQuestionChoiceUpdateRequest : SelectableQuestionChoiceCreateRequest
{
    public int? SelectableQuestionChoiceId { get; set; }
}
