CREATE PROCEDURE dbo.MilestonePersonsByMilestoneForTEByProject
(
	@MilestoneId INT
)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM Milestone WHERE MilestoneId = @MilestoneId)
	BEGIN
		SELECT 
			mp.MilestonePersonId,
	       mp.PersonId,
	       mp.FirstName,
	       mp.LastName 
		FROM dbo.v_MilestonePerson AS mp
		WHERE mp.MilestoneId = @MilestoneId
	END
	ELSE
	BEGIN
		--DECLARE @MileStonePersons
		DECLARE @StartDate DATETIME,
				@EndDate DATETIME,
				@MilestoneIdLocal INT
		SELECT @MilestoneIdLocal = MilestoneId
		FROM dbo.DefaultMilestoneSetting 
		 
		SELECT @StartDate = CONVERT(DATETIME,CONVERT(NVARCHAR,@MilestoneId))
		SELECT @EndDate = DATEADD(MM,1,@StartDate)-1
		
		SELECT 
			mp.MilestonePersonId,
			mp.PersonId,
	       mp.FirstName,
	       mp.LastName 
		FROM v_MilestonePerson mp
		WHERE mp.MilestoneId = @MilestoneIdLocal
			AND (@StartDate BETWEEN StartDate AND EndDate
				OR @EndDate BETWEEN StartDate AND EndDate)
		
	END

END
