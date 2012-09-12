CREATE PROCEDURE dbo.GetPersonMilestonesAfterTerminationDate
(
	@PersonId	INT,
	@TerminationDate	DATETIME 
)
AS
BEGIN 
	
	SELECT DISTINCT
			ProjectId,
			ProjectNumber,
			ProjectName,
			MilestoneId,
			MilestoneName,
			M.MilestoneStartDate			
	FROM dbo.v_MilestonePerson  M
	WHERE M.PersonId = @PersonId
			AND EndDate >= @TerminationDate
			AND M.MilestoneId NOT IN (SELECT  MilestoneId FROM DefaultMilestoneSetting)
	ORDER BY ProjectNumber,M.MilestoneStartDate
END
