CREATE PROCEDURE [dbo].[PersonsGetBySeniorityAndStatus]
	@SeniorityId int, 
	@PersonStatusId    int = NULL
AS
	SELECT PersonId,
		   FirstName,
		   LastName,
		   IsDefaultManager,
		   HireDate
	FROM dbo.Person
	WHERE SeniorityId = @SeniorityId
			AND (PersonStatusId = @PersonStatusId OR @PersonStatusId IS NULL)

