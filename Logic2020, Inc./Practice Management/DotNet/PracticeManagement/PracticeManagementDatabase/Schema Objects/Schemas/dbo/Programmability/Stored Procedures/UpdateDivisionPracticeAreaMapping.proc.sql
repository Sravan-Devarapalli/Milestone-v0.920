CREATE PROCEDURE [dbo].[UpdateDivisionPracticeAreaMapping]
	@PracticeId INT,
	@DivisionIds NVARCHAR(MAX) = NULL
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @DivisionIdsTable TABLE (Id int)
	
	IF(@DivisionIds IS NULL)
	BEGIN
		INSERT INTO @DivisionIdsTable(Id)
		SELECT PD.DivisionId
			FROM PersonDivision PD 
			WHERE PD.Inactive=0
	END

	IF(@DivisionIds IS NOT NULL AND @DivisionIds<>'')
	BEGIN
		INSERT INTO @DivisionIdsTable(Id)
			SELECT D.ResultId
			FROM dbo.ConvertStringListIntoTable(@DivisionIds) D
	END

	DELETE FROM DivisionPracticeArea 
		   WHERE PracticeId=@PracticeId

	INSERT INTO DivisionPracticeArea(Division_Practice_Id,DivisionId,PracticeId)
	SELECT NEWID(), D.Id, @PracticeId 
	FROM @DivisionIdsTable AS D 

END
