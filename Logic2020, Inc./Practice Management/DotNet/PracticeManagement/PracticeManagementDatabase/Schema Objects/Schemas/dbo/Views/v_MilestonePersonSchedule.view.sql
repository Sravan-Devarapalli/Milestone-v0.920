CREATE VIEW dbo.v_MilestonePersonSchedule WITH SCHEMABINDING
AS
	SELECT m.[MilestoneId],
	       mp.PersonId,
	       CASE
	           WHEN cal.Date BETWEEN mpe.StartDate AND ISNULL(mpe.EndDate, m.[ProjectedDeliveryDate])
	           THEN mpe.HoursPerDay
	           ELSE 0
	       END AS HoursPerDay,
	       cal.Date,
	       m.ProjectId,
	       mpe.StartDate AS EntryStartDate,
	       mpe.Amount,
	       mpe.PersonRoleId,
	       m.IsHourlyAmount,
	       m.StartDate,
	       m.ProjectedDeliveryDate,
	       m.IsDefault IsDefaultMileStone
	  FROM dbo.[Milestone] AS m
		   INNER JOIN dbo.MilestonePerson AS mp ON m.[MilestoneId] = mp.[MilestoneId]
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
	       INNER JOIN dbo.PersonCalendarAuto AS cal
	           ON cal.Date BETWEEN mpe.Startdate AND ISNULL(mpe.EndDate, m.ProjectedDeliveryDate) AND cal.PersonId = mp.PersonId
	 WHERE cal.DayOff = 0

