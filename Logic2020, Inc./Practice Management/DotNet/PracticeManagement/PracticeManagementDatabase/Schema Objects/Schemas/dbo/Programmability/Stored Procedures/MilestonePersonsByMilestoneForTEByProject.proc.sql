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
	       p.FirstName,
	       p.LastName 
		FROM dbo.MilestonePerson AS mp
		INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId AND p.IsStrawman = 0
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
		FROM dbo.v_MilestonePerson AS mp
		INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId AND p.IsStrawman = 0
		WHERE mp.MilestoneId = @MilestoneIdLocal
			AND (@StartDate BETWEEN StartDate AND EndDate
				OR @EndDate BETWEEN StartDate AND EndDate)
			AND (EXISTS (
						SELECT 1 
						FROM dbo.PersonStatusHistory PH
						WHERE PH.PersonId = mp.PersonId
								AND PH.PersonStatusId = 1 --Active
								AND (@StartDate BETWEEN PH.StartDate AND ISNULL(PH.EndDate,dbo.GetFutureDate())
									OR @EndDate BETWEEN PH.StartDate AND ISNULL(PH.EndDate,dbo.GetFutureDate())
									OR PH.StartDate BETWEEN @StartDate AND @EndDate
									OR (PH.EndDate BETWEEN @StartDate AND @EndDate AND PH.EndDate IS NOT NULL)
									)
									)
				OR EXISTS(
						SELECT 1 FROM dbo.TimeEntries TE
						WHERE TE.MilestonePersonId = mp.MilestonePersonId
						AND TE.MilestoneDate BETWEEN @StartDate AND @EndDate
				)
			)
	END

END
