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

    public int? ParentOptionId { get; set; }

    public List<QuestionOptionCreateRequest> Options { get; set; } = [];
}

public class QuestionUpdateRequest : QuestionCreateRequest
{
    public int? QuestionId { get; set; }

    public new List<QuestionOptionUpdateRequest> Options { get; set; } = [];
}

public class QuestionOptionCreateRequest
{
    [Required]
    [MaxLength(500)]
    public string OptionText { get; set; } = null!;

    public int DisplayOrder { get; set; }
}

public class QuestionOptionUpdateRequest : QuestionOptionCreateRequest
{
    public int? QuestionOptionId { get; set; }
}
