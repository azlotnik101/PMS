using PMS.Entities;
using PMS.Models.Responses;

namespace PMS.Services;

public class QuestionnaireResponseFactory
{
    public QuestionnaireDto ToResponse(Questionnaire questionnaire)
    {
        return new QuestionnaireDto
        {
            QuestionnaireId = questionnaire.QuestionnaireId,
            Title = questionnaire.Title,
            Questions = [.. questionnaire.Questions
                .OrderBy(question => question.DisplayOrder)
                .Select(ToResponse)]
        };
    }

    private static QuestionDto ToResponse(Question question)
    {
        return new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionnaireId = question.QuestionnaireId,
            Text = question.Text,
            QuestionType = question.QuestionType,
            DisplayOrder = question.DisplayOrder,
            ParentQuestionId = question.ParentQuestionId,
            ParentChoiceId = question.ParentChoiceId,
            Choices = [.. question.Choices
                .OrderBy(choice => choice.DisplayOrder)
                .Select(ToResponse)]
        };
    }

    private static SelectableQuestionChoiceDto ToResponse(SelectableQuestionChoice choice)
    {
        return new SelectableQuestionChoiceDto
        {
            SelectableQuestionChoiceId = choice.SelectableQuestionChoiceId,
            QuestionId = choice.QuestionId,
            ChoiceText = choice.ChoiceText,
            DisplayOrder = choice.DisplayOrder
        };
    }
}
