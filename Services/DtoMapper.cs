using System.Text.Json;
using PMS.Entities;
using PMS.Models.Responses;

namespace PMS.Services;

public static class DtoMapper
{
    public static ParticipantDto ToDto(Participant participant)
    {
        return new ParticipantDto
        {
            ParticipantId = participant.ParticipantId,
            FirstName = participant.FirstName,
            LastName = participant.LastName,
            EmailAddress = participant.EmailAddress,
            DateOfBirth = participant.DateOfBirth,
            ParticipantCode = participant.ParticipantCode,
            StartDate = participant.StartDate
        };
    }

    public static QuestionnaireDto ToDto(Questionnaire questionnaire)
    {
        return new QuestionnaireDto
        {
            QuestionnaireId = questionnaire.QuestionnaireId,
            Title = questionnaire.Title,
            Questions = [.. questionnaire.Questions
                .OrderBy(question => question.DisplayOrder)
                .Select(ToDto)]
        };
    }

    public static QuestionDto ToDto(Question question)
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
                .Select(ToDto)]
        };
    }

    public static SelectableQuestionChoiceDto ToDto(SelectableQuestionChoice choice)
    {
        return new SelectableQuestionChoiceDto
        {
            SelectableQuestionChoiceId = choice.SelectableQuestionChoiceId,
            QuestionId = choice.QuestionId,
            ChoiceText = choice.ChoiceText,
            DisplayOrder = choice.DisplayOrder
        };
    }

    public static QuestionnaireAssignmentDto ToDto(QuestionnaireAssignment assignment)
    {
        return new QuestionnaireAssignmentDto
        {
            QuestionnaireAssignmentId = assignment.QuestionnaireAssignmentId,
            ParticipantId = assignment.ParticipantId,
            QuestionnaireId = assignment.QuestionnaireId,
            AssignedDate = assignment.AssignedDate,
            Completed = assignment.Completed,
            Participant = assignment.Participant is null ? null : ToDto(assignment.Participant),
            Questionnaire = assignment.Questionnaire is null ? null : ToDto(assignment.Questionnaire)
        };
    }

    public static QuestionAnswerDto ToDto(QuestionResponse response)
    {
        return new QuestionAnswerDto
        {
            QuestionResponseId = response.QuestionResponseId,
            QuestionnaireAssignmentId = response.QuestionnaireAssignmentId,
            QuestionId = response.QuestionId,
            TextValue = response.TextValue,
            NumericValue = response.NumericValue,
            AnsweredDate = response.AnsweredDate,
            SelectedQuestionChoiceIds = string.IsNullOrWhiteSpace(response.SelectedChoiceIds)
                ? []
                : JsonSerializer.Deserialize<List<int>>(response.SelectedChoiceIds) ?? []
        };
    }

    public static AuditLogDto ToDto(AuditLog auditLog)
    {
        return new AuditLogDto
        {
            AuditLogId = auditLog.AuditLogId,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            Action = auditLog.Action,
            Details = auditLog.Details,
            CreatedDate = auditLog.CreatedDate
        };
    }
}
