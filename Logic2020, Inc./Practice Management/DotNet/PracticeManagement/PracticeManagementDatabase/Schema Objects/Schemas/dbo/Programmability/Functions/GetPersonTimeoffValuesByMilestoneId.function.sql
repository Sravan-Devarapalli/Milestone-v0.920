CREATE FUNCTION [dbo].[GetPersonTimeoffValuesByMilestoneId]
(
	@ProjectId           INT =null,
	@MilestoneId         INT = null,
	@MilestonePersonId   INT = Null,
	@MilestonePersonEntryId   INT = null
)
RETURNS TABLE
AS
	RETURN

		SELECT distinct mp.personid,pcal.Date,CONVERT(DECIMAL(10,2),ISNULL(pcal.ActualHours,0)) AS ActualHours
		FROM  dbo.Milestone AS m
			  INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestoneId = m.MilestoneId 
													AND (@ProjectId IS NULL OR m.ProjectId = @ProjectId)
													AND ( @MilestoneId IS NULL OR m.MilestoneId = @MilestoneId )
			  INNER JOIN dbo.MilestonePersonEntry AS mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
														AND (@MilestonePersonId IS NULL OR @MilestonePersonId = mp.MilestonePersonId)
														AND (@MilestonePersonEntryId IS NULL OR @MilestonePersonEntryId = mpe.Id)
			  INNER JOIN dbo.v_PersonCalendar AS pcal ON pcal.DayOff = 1 AND pcal.CompanyDayOff = 0 --person Time-off day
													AND pcal.Date BETWEEN mpe.StartDate AND mpe.EndDate
													AND pcal.PersonId = mp.PersonId
													AND Pcal.IsFloatingHoliday = 0
	
