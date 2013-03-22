CREATE VIEW [dbo].[v_MilestonePersonSchedule] --Entry Level
AS
	SELECT m.[MilestoneId],
	       mp.PersonId,
	      -- dbo.PersonProjectedHoursPerDay(cal.DayOff,cal.companydayoff,cal.TimeoffHours,mpe.HoursPerDay) AS HoursPerDay,
		  --Removed Inline Function for the sake of performance. When ever PersonProjectedHoursPerDay function is updated need to update below case when also.
			CONVERT(DECIMAL(4,2),CASE WHEN cal.DayOff = 0  THEN mpe.HoursPerDay -- No Time-off and no company holiday
				WHEN (cal.DayOff = 1 and cal.companydayoff = 1) OR (cal.DayOff = 1 AND cal.companydayoff = 0 AND ISNULL(cal.TimeoffHours,8) = 8) THEN 0 -- only company holiday OR person complete dayoff
				ELSE mpe.HoursPerDay * (1-(cal.TimeoffHours/8)) --person partial day off
			END) AS HoursPerDay,
	       cal.Date,
	       m.ProjectId,
	       mpe.Id EntryId,
		   mpe.StartDate AS EntryStartDate,
	       mpe.Amount,
	       mpe.PersonRoleId,
	       m.IsHourlyAmount,
	       m.StartDate,
	       m.ProjectedDeliveryDate,
		   P.ProjectStatusId
	  FROM dbo.Project P 
		   INNER JOIN dbo.[Milestone] AS m ON m.ProjectId=P.ProjectId AND P.IsAdministrative = 0 AND P.ProjectId != 174  AND m.IsDefault = 0
		   INNER JOIN dbo.MilestonePerson AS mp ON mp.[MilestoneId] = m.[MilestoneId]
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.PersonCalendarAuto AS cal ON cal.Date BETWEEN mpe.Startdate AND mpe.EndDate AND mp.PersonId = cal.PersonId

