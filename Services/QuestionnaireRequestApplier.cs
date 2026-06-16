using PMS.Entities;
using PMS.Models.Requests;

namespace PMS.Services;

public class QuestionnaireRequestApplier
{
    public Questionnaire CreateQuestionnaire(QuestionnaireCreateRequest request)
    {
        var questionnaire = new Questionnaire();
        Apply(questionnaire, request);
        return questionnaire;
    }

    public void Apply(Questionnaire questionnaire, QuestionnaireCreateRequest request)
    {
        questionnaire.Title = request.Title;
        questionnaire.Questions = [.. request.Questions.Select(CreateQuestion)];
    }

    public void Apply(Questionnaire questionnaire, QuestionnaireUpdateRequest request)
    {
        questionnaire.Title = request.Title;
    }

    public Question CreateQuestion(QuestionCreateRequest request)
    {
        var question = new Question();
        Apply(question, request);
        question.Choices = [.. request.Choices.Select(CreateChoice)];
        return question;
    }

    public Question CreateQuestion(QuestionUpdateRequest request)
    {
        var question = new Question();
        Apply(question, request);
        question.Choices = [.. request.Choices.Select(CreateChoice)];
        return question;
    }

    public void Apply(Question question, QuestionCreateRequest request)
    {
        question.Text = request.Text;
        question.QuestionType = request.QuestionType;
        question.DisplayOrder = request.DisplayOrder;
        question.ParentQuestionId = request.ParentQuestionId;
        question.ParentChoiceId = request.ParentChoiceId;
    }

    public SelectableQuestionChoice CreateChoice(SelectableQuestionChoiceCreateRequest request)
    {
        var choice = new SelectableQuestionChoice();
        Apply(choice, request);
        return choice;
    }

    public SelectableQuestionChoice CreateChoice(SelectableQuestionChoiceUpdateRequest request)
    {
        var choice = new SelectableQuestionChoice();
        Apply(choice, request);
        return choice;
    }

    public void Apply(SelectableQuestionChoice choice, SelectableQuestionChoiceCreateRequest request)
    {
        choice.ChoiceText = request.ChoiceText;
        choice.DisplayOrder = request.DisplayOrder;
    }
}
