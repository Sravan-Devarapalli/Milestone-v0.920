CREATE PROCEDURE [dbo].[SavePracticesIsNotesRequiredDetails]
	@NotesRequiredPracticeIdsList NVARCHAR(250), 
	@NotesExemptedPracticeIdsList NVARCHAR(250)
AS
BEGIN
	
	UPDATE dbo.Practice 
	SET IsNotesRequired = 1
	WHERE PracticeId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@NotesRequiredPracticeIdsList))

	UPDATE dbo.Practice 
	SET IsNotesRequired = 0
	WHERE PracticeId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@NotesExemptedPracticeIdsList))

END

