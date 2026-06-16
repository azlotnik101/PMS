using System.ComponentModel.DataAnnotations;

namespace PMS.Models.Requests;

public class QuestionResponseCreateRequest
{
    [Range(1, int.MaxValue)]
    public int QuestionnaireAssignmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int QuestionId { get; set; }

    public string? TextValue { get; set; }

    public decimal? NumericValue { get; set; }

    public List<int> SelectedQuestionOptionIds { get; set; } = [];
}
