

CREATE PROCEDURE dbo.MilestonePersonEntryListByMilestonePersonId
(
	@MilestonePersonId   INT
)
AS
	SET NOCOUNT ON

	SELECT mp.MilestonePersonId,
		   mp.SeniorityId,
           mp.PersonId,
	       mp.StartDate,
	       mp.EndDate,
	       mp.PersonRoleId,
	       mp.Amount,
	       mp.HoursPerDay,
	       mp.RoleName,
		   mp.VacationDays,	
	       mp.ExpectedHours,
		   mp.Location
	  FROM dbo.v_MilestonePerson AS mp
	 WHERE mp.MilestonePersonId = @MilestonePersonId

