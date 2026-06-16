using PMS.Data;
using PMS.Entities;
using PMS.Models.Requests;

namespace PMS.Services;

public class QuestionnaireSynchronizer(PmsDbContext dbContext, QuestionnaireRequestApplier requestApplier)
{
    public void SyncQuestions(Questionnaire questionnaire, List<QuestionUpdateRequest> requests)
    {
        var requestedQuestionIds = requests
            .Where(question => question.QuestionId.HasValue)
            .Select(question => question.QuestionId!.Value)
            .ToHashSet();

        var questionsToRemove = questionnaire.Questions
            .Where(question => !requestedQuestionIds.Contains(question.QuestionId))
            .ToList();

        dbContext.Questions.RemoveRange(questionsToRemove);

        foreach (var request in requests)
        {
            SyncQuestion(questionnaire, request);
        }
    }

    private void SyncQuestion(Questionnaire questionnaire, QuestionUpdateRequest request)
    {
        var question = request.QuestionId.HasValue
            ? questionnaire.Questions.FirstOrDefault(existingQuestion => existingQuestion.QuestionId == request.QuestionId.Value)
            : null;

        if (question is null)
        {
            questionnaire.Questions.Add(requestApplier.CreateQuestion(request));
            return;
        }

        requestApplier.Apply(question, request);
        SyncChoices(question, request.Choices);
    }

    private void SyncChoices(Question question, List<SelectableQuestionChoiceUpdateRequest> requests)
    {
        var requestedChoiceIds = requests
            .Where(choice => choice.SelectableQuestionChoiceId.HasValue)
            .Select(choice => choice.SelectableQuestionChoiceId!.Value)
            .ToHashSet();

        var choicesToRemove = question.Choices
            .Where(choice => !requestedChoiceIds.Contains(choice.SelectableQuestionChoiceId))
            .ToList();

        dbContext.SelectableQuestionChoices.RemoveRange(choicesToRemove);

        foreach (var request in requests)
        {
            SyncChoice(question, request);
        }
    }

    private void SyncChoice(Question question, SelectableQuestionChoiceUpdateRequest request)
    {
        var choice = request.SelectableQuestionChoiceId.HasValue
            ? question.Choices.FirstOrDefault(existingChoice => existingChoice.SelectableQuestionChoiceId == request.SelectableQuestionChoiceId.Value)
            : null;

        if (choice is null)
        {
            question.Choices.Add(requestApplier.CreateChoice(request));
            return;
        }

        requestApplier.Apply(choice, request);
    }
}
