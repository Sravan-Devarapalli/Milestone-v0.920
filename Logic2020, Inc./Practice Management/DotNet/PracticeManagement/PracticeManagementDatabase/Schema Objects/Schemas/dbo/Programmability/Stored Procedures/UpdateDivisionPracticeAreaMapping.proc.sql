CREATE PROCEDURE [dbo].[UpdateDivisionPracticeAreaMapping]
	@PracticeId INT,
	@DivisionIds NVARCHAR(MAX) = NULL,
	@ProjectDivisionIds NVARCHAR(MAX) = NULL
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @DivisionIdsTable TABLE (Id int)
	DECLARE @ProjectDivisionIdsTable TABLE (Id int)
	
	IF(@DivisionIds IS NULL)
	BEGIN
		INSERT INTO @DivisionIdsTable(Id)
		SELECT PD.DivisionId
			FROM PersonDivision PD 
			WHERE PD.Inactive=0
	END

	IF(@ProjectDivisionIds IS NULL)
	BEGIN
		INSERT INTO @ProjectDivisionIdsTable(Id)
		SELECT PD.DivisionId
			FROM ProjectDivision PD 
	END


	IF(@DivisionIds IS NOT NULL AND @DivisionIds<>'')
	BEGIN
		INSERT INTO @DivisionIdsTable(Id)
			SELECT D.ResultId
			FROM dbo.ConvertStringListIntoTable(@DivisionIds) D
	END

	IF(@ProjectDivisionIds IS NOT NULL AND @ProjectDivisionIds<>'')
	BEGIN
		INSERT INTO @ProjectDivisionIdsTable(Id)
			SELECT D.ResultId
			FROM dbo.ConvertStringListIntoTable(@ProjectDivisionIds) D
	END

	--person division practice area mappings
	DELETE FROM DivisionPracticeArea 
		   WHERE PracticeId=@PracticeId

	INSERT INTO DivisionPracticeArea(Division_Practice_Id,DivisionId,PracticeId)
	SELECT NEWID(), D.Id, @PracticeId 
	FROM @DivisionIdsTable AS D 

	--Project division practice area mappings
	DELETE FROM ProjectDivisionPracticeMapping
			WHERE PracticeId=@PracticeId

	
	INSERT INTO ProjectDivisionPracticeMapping(ProjectDivision_Practice_Id,ProjectDivisionId,PracticeId)
	SELECT NEWID(), D.Id, @PracticeId 
	FROM @ProjectDivisionIdsTable AS D 

END

