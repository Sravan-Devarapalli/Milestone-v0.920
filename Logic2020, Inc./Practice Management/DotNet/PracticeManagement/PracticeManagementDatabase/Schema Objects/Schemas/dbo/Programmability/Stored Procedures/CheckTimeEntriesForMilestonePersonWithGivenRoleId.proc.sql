CREATE PROCEDURE [dbo].[CheckTimeEntriesForMilestonePersonWithGivenRoleId]
	@MilestonePersonId INT,
	@PersonRoleId INT = NULL
AS

DECLARE @EntriesCount INT

SET @PersonRoleId = ISNULL(@PersonRoleId,0)

;WITH CTE
	AS
	(  
		SELECT  ISNULL(mpe.PersonRoleId,0) as PersonRoleId,te.TimeEntryId
			FROM dbo.TimeEntries AS te
			INNER JOIN  dbo.MilestonePersonEntry AS mpe 
			ON MPE.MilestonePersonId = TE.MilestonePersonId  
			AND  TE.MilestoneDate BETWEEN MPE.StartDate AND MPE.EndDate AND mpe.MilestonePersonId = @MilestonePersonId  
	)


SELECT @EntriesCount =  MIN(CountOfMpeApplicable)
FROM (  
		SELECT COUNT(CTE2.TimeEntryId) as CountOfMpeApplicable 
		FROM (SELECT * FROM CTE WHERE PersonRoleId = @PersonRoleId) AS CTE1 LEFT JOIN (SELECT * FROM CTE WHERE PersonRoleId != @PersonRoleId)  AS CTE2 
		ON CTE1.TimeEntryId = CTE2.TimeEntryId	
		GROUP BY CTE1.TimeEntryId
		) AS CTE3


	IF (@EntriesCount = 0)
		BEGIN
			SELECT CONVERT(BIT,1) Result 
		END
		ELSE
		BEGIN
			SELECT CONVERT(BIT,0) Result
		END


