CREATE PROCEDURE [dbo].[GetPersonsByPayTypesAndByStatusIds]
(
	@StatusIds	NVARCHAR(MAX),
	@PayTypeIds	NVARCHAR(MAX)
)
AS
BEGIN

	DECLARE @StatusIdTable TABLE
	(
		StatusId INT
	)
	DECLARE @PayTypeIdTable TABLE
	(
		PayTypeId INT
	)
	DECLARE @Today DATE 

	SELECT @Today = dbo.GettingPMTime(GETUTCDATE());
	
	INSERT INTO @StatusIdTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@StatusIds)

	INSERT INTO @PayTypeIdTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PayTypeIds)

	SELECT P.PersonId,P.FirstName,P.LastName,P.IsDefaultManager
	FROM dbo.Person P
	INNER JOIN Pay pay ON pay.Person = P.PersonId
	WHERE P.PersonStatusId IN (SELECT StatusId FROM @StatusIdTable)
	AND @Today BETWEEN pay.StartDate AND pay.EndDate AND  
	pay.Timescale IN (SELECT PayTypeId FROM @PayTypeIdTable)
	AND P.IsStrawman = 0

END

