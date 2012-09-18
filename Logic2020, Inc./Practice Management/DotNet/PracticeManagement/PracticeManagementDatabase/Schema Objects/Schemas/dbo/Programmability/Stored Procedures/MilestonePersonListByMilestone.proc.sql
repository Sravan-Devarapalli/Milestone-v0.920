CREATE PROCEDURE [dbo].[MilestonePersonListByMilestone]
(
	@MilestoneId   INT
)
AS
	SET NOCOUNT ON
	DECLARE @MilestoneIdLocal INT,@FutureDate DATETIME
	SELECT @MilestoneIdLocal = @MilestoneId,@FutureDate = dbo.GetFutureDate()

	;WITH MileStonePersonActiveDates
	AS 
	(
		SELECT PH.PersonId,MIN(PH.HireDate) AS FirstHireDate,MAX(ISNULL(PH.TerminationDate,@FutureDate)) AS LastTerminationDate
		FROM dbo.v_PersonHistory PH  
		INNER JOIN dbo.MilestonePerson MP ON MP.PersonId = PH.PersonId	AND MP.MilestoneId = @MilestoneIdLocal
		GROUP BY PH.PersonId
	)

	SELECT mp.MilestonePersonId,
	       mp.MilestoneId,
	       mp.PersonId,
	       mp.SeniorityId,
	       mp.StartDate,
	       mp.EndDate,
	       mp.HoursPerDay,
	       mp.FirstName,
	       mp.LastName,
	       mp.ProjectId,
	       mp.ProjectName,
	       mp.ProjectStartDate,
	       mp.ProjectEndDate,
	       mp.Discount,
	       mp.ClientId,
	       mp.ClientName,
	       mp.MilestoneName,
	       mp.MilestoneStartDate,
	       mp.MilestoneProjectedDeliveryDate,
	       mp.ExpectedHours,
	       mp.PersonRoleId,
	       mp.RoleName,
	       mp.Amount,
	       mp.IsHourlyAmount,
	       mp.SalesCommission,
	       mp.MilestoneExpectedHours,
	       mp.MilestoneHourlyRevenue,
	       p.HireDate,
	       p.TerminationDate,
		   mp.EntryId,
		   MPAD.FirstHireDate,
		   MPAD.LastTerminationDate
	  FROM dbo.v_MilestonePerson AS mp
	  INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId
	  INNER JOIN MileStonePersonActiveDates AS MPAD ON MPAD.PersonId = P.PersonId
	  WHERE mp.MilestoneId = @MilestoneIdLocal

