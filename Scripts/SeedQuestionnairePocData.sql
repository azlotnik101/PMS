USE [PMS];
GO

DECLARE @QuestionnaireId int;
DECLARE @HoursQuestionId int;
DECLARE @DevicesQuestionId int;
DECLARE @TabletBrandQuestionId int;
DECLARE @TextQuestionId int;
DECLARE @TabletOptionId int;

-- QuestionType enum values:
-- Numeric = 1
-- Text = 2
-- SingleSelect = 3
-- MultiSelect = 4

IF NOT EXISTS (SELECT 1 FROM Questionnaires WHERE Title = 'Online Usage Survey')
BEGIN
    INSERT INTO Questionnaires (Title)
    VALUES ('Online Usage Survey');

    SET @QuestionnaireId = SCOPE_IDENTITY();

    INSERT INTO AuditLogs (EntityName, EntityId, Action, Details, CreatedDate)
    VALUES ('Questionnaire', @QuestionnaireId, 'Created', 'Seeded Online Usage Survey questionnaire.', SYSUTCDATETIME());
END
ELSE
BEGIN
    SELECT @QuestionnaireId = QuestionnaireId
    FROM Questionnaires
    WHERE Title = 'Online Usage Survey';
END;

IF NOT EXISTS (SELECT 1 FROM Questions WHERE QuestionnaireId = @QuestionnaireId AND Text = 'How many hours per day do you spend online?')
BEGIN
    INSERT INTO Questions (QuestionnaireId, Text, QuestionType, DisplayOrder, ParentQuestionId, ParentOptionId)
    VALUES (@QuestionnaireId, 'How many hours per day do you spend online?', 1, 1, NULL, NULL);
END;

SELECT @HoursQuestionId = QuestionId
FROM Questions
WHERE QuestionnaireId = @QuestionnaireId
  AND Text = 'How many hours per day do you spend online?';

IF NOT EXISTS (SELECT 1 FROM Questions WHERE QuestionnaireId = @QuestionnaireId AND Text = 'Which of the following devices do you use?')
BEGIN
    INSERT INTO Questions (QuestionnaireId, Text, QuestionType, DisplayOrder, ParentQuestionId, ParentOptionId)
    VALUES (@QuestionnaireId, 'Which of the following devices do you use?', 4, 2, NULL, NULL);
END;

SELECT @DevicesQuestionId = QuestionId
FROM Questions
WHERE QuestionnaireId = @QuestionnaireId
  AND Text = 'Which of the following devices do you use?';

IF NOT EXISTS (SELECT 1 FROM QuestionOptions WHERE QuestionId = @DevicesQuestionId AND OptionText = 'Smartphone')
BEGIN
    INSERT INTO QuestionOptions (QuestionId, OptionText, DisplayOrder)
    VALUES (@DevicesQuestionId, 'Smartphone', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QuestionOptions WHERE QuestionId = @DevicesQuestionId AND OptionText = 'Laptop')
BEGIN
    INSERT INTO QuestionOptions (QuestionId, OptionText, DisplayOrder)
    VALUES (@DevicesQuestionId, 'Laptop', 2);
END;

IF NOT EXISTS (SELECT 1 FROM QuestionOptions WHERE QuestionId = @DevicesQuestionId AND OptionText = 'Tablet')
BEGIN
    INSERT INTO QuestionOptions (QuestionId, OptionText, DisplayOrder)
    VALUES (@DevicesQuestionId, 'Tablet', 3);
END;

SELECT @TabletOptionId = QuestionOptionId
FROM QuestionOptions
WHERE QuestionId = @DevicesQuestionId
  AND OptionText = 'Tablet';

IF NOT EXISTS (SELECT 1 FROM Questions WHERE QuestionnaireId = @QuestionnaireId AND Text = 'Which tablet brand do you use?')
BEGIN
    INSERT INTO Questions (QuestionnaireId, Text, QuestionType, DisplayOrder, ParentQuestionId, ParentOptionId)
    VALUES (@QuestionnaireId, 'Which tablet brand do you use?', 3, 3, @DevicesQuestionId, @TabletOptionId);
END
ELSE
BEGIN
    UPDATE Questions
    SET ParentQuestionId = @DevicesQuestionId,
        ParentOptionId = @TabletOptionId,
        QuestionType = 3,
        DisplayOrder = 3
    WHERE QuestionnaireId = @QuestionnaireId
      AND Text = 'Which tablet brand do you use?';
END;

SELECT @TabletBrandQuestionId = QuestionId
FROM Questions
WHERE QuestionnaireId = @QuestionnaireId
  AND Text = 'Which tablet brand do you use?';

IF NOT EXISTS (SELECT 1 FROM SelectableQuestionChoices WHERE QuestionId = @TabletBrandQuestionId AND OptionText = 'Apple')
BEGIN
    INSERT INTO SelectableQuestionChoices (QuestionId, OptionText, DisplayOrder)
    VALUES (@TabletBrandQuestionId, 'Apple', 1);
END;

IF NOT EXISTS (SELECT 1 FROM SelectableQuestionChoices WHERE QuestionId = @TabletBrandQuestionId AND OptionText = 'Samsung')
BEGIN
    INSERT INTO SelectableQuestionChoices (QuestionId, OptionText, DisplayOrder)
    VALUES (@TabletBrandQuestionId, 'Samsung', 2);
END;

IF NOT EXISTS (SELECT 1 FROM SelectableQuestionChoices WHERE QuestionId = @TabletBrandQuestionId AND OptionText = 'Microsoft Surface')
BEGIN
    INSERT INTO SelectableQuestionChoices (QuestionId, OptionText, DisplayOrder)
    VALUES (@TabletBrandQuestionId, 'Microsoft Surface', 3);
END;

IF NOT EXISTS (SELECT 1 FROM SelectableQuestionChoices WHERE QuestionId = @TabletBrandQuestionId AND OptionText = 'Lenovo')
BEGIN
    INSERT INTO SelectableQuestionChoices (QuestionId, OptionText, DisplayOrder)
    VALUES (@TabletBrandQuestionId, 'Lenovo', 4);
END;

IF NOT EXISTS (SELECT 1 FROM SelectableQuestionChoices WHERE QuestionId = @TabletBrandQuestionId AND OptionText = 'Huawei')
BEGIN
    INSERT INTO SelectableQuestionChoices (QuestionId, OptionText, DisplayOrder)
    VALUES (@TabletBrandQuestionId, 'Huawei', 5);
END;

IF NOT EXISTS (SELECT 1 FROM Questions WHERE QuestionnaireId = @QuestionnaireId AND Text = 'What improvements would you like?')
BEGIN
    INSERT INTO Questions (QuestionnaireId, Text, QuestionType, DisplayOrder, ParentQuestionId, ParentOptionId)
    VALUES (@QuestionnaireId, 'What improvements would you like?', 2, 4, NULL, NULL);
END;

SELECT @TextQuestionId = QuestionId
FROM Questions
WHERE QuestionnaireId = @QuestionnaireId
  AND Text = 'What improvements would you like?';

SELECT
    qn.QuestionnaireId,
    qn.Title,
    q.QuestionId,
    q.Text AS QuestionText,
    CASE q.QuestionType
        WHEN 1 THEN 'Numeric'
        WHEN 2 THEN 'Text'
        WHEN 3 THEN 'SingleSelect'
        WHEN 4 THEN 'MultiSelect'
    END AS QuestionType,
    q.DisplayOrder,
    q.ParentQuestionId,
    q.ParentOptionId,
    qo.QuestionOptionId,
    qo.OptionText,
    qo.DisplayOrder AS OptionDisplayOrder
FROM Questionnaires qn
INNER JOIN Questions q
    ON qn.QuestionnaireId = q.QuestionnaireId
LEFT JOIN SelectableQuestionChoices qo
    ON q.QuestionId = qo.QuestionId
WHERE qn.QuestionnaireId = @QuestionnaireId
ORDER BY q.DisplayOrder, qo.DisplayOrder;
GO
