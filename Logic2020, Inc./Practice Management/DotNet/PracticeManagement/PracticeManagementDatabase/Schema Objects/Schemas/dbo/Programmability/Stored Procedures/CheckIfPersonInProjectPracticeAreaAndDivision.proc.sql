CREATE PROCEDURE [dbo].[CheckIfPersonInProjectPracticeAreaAndDivision]
(	
	@PersonId INT
)
AS
BEGIN 
	DECLARE @isDivisionOrPracticeOwner BIT =CAST(0 AS BIT),
			@isAssignedToProject BIT = CAST(0 AS BIT)
	
	IF EXISTS(SELECT 1 FROM dbo.PersonDivision  WHERE DivisionOwnerId=@PersonId
	                   UNION ALL
	                   SELECT 1 FROM dbo.Practice WHERE PracticeManagerId=@PersonId)
	SET @isDivisionOrPracticeOwner =CAST(1 AS BIT)
	
	IF EXISTS(SELECT 1 FROM dbo.v_MilestonePerson M 
						JOIN dbo.Person P ON P.PersonId = M.PersonId 
						WHERE M.PersonId = @PersonId
						AND EndDate >= P.HireDate
						AND M.MilestoneId NOT IN (SELECT  MilestoneId FROM DefaultMilestoneSetting))

				SET @isAssignedToProject = CAST(1 AS BIT)

	SELECT @isDivisionOrPracticeOwner as 'IsDivisionOrPracticeOwner',
		   @isAssignedToProject as 'IsAssignedToProject'
END
