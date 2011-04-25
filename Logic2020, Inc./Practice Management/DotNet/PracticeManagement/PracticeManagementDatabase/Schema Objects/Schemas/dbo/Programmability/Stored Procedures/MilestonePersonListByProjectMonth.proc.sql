

CREATE PROCEDURE [dbo].[MilestonePersonListByProjectMonth]
(
	@ProjectId   INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON
	
	SELECT mp.MilestoneId,
	       mp.PersonId,
	       mp.SeniorityId,
	       mh.[Month] AS StartDate,
	       mh.[MonthEnd] AS EndDate,
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
	       ISNULL(mh.MilestonePersonMonthHours, 0) AS ExpectedHours,
	       mp.PersonRoleId,
	       mp.RoleName,
	       mp.Amount,
	       mp.IsHourlyAmount,
	       mp.SalesCommission,
	       mp.MilestoneExpectedHours,
	       mp.MilestoneActualDeliveryDate,
	       mp.MilestoneHourlyRevenue
	  FROM dbo.v_MilestonePerson AS mp
	       INNER JOIN dbo.v_MilestonePersonMonthHours AS mh
	           ON mh.MilestoneId = mp.MilestoneId AND mh.PersonId = mp.PersonId
	 WHERE mp.ProjectId = @ProjectId
	   AND mh.[Month] BETWEEN @StartDate AND @EndDate
	   AND mh.MilestonePersonMonthHours <> 0
	ORDER BY mp.MilestoneId, mp.PersonId

