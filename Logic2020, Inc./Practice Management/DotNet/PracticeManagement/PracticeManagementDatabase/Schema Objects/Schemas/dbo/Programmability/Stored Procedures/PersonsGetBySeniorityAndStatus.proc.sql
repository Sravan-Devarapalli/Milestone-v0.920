CREATE PROCEDURE [dbo].[PersonsGetBySeniorityAndStatus]
	@SeniorityId int, 
	@PersonStatusId    int = NULL
AS
	SELECT PersonId,
		   FirstName,
		   LastName,
		   IsDefaultManager,
		   HireDate
	FROM dbo.Person P
	JOIN dbo.Seniority S ON P.SeniorityId = S.SeniorityId
	WHERE S.SeniorityValue = @SeniorityId
			AND (PersonStatusId = @PersonStatusId OR @PersonStatusId IS NULL)

