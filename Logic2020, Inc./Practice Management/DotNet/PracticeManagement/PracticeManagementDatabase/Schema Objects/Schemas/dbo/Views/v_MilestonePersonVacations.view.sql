CREATE VIEW dbo.v_MilestonePersonVacations
AS  
	SELECT  m.MilestoneId,
			mp.MilestonePersonId,
			mp.PersonId,
			mpe.StartDate,
			mpe.EndDate,
			ISNULL(SUM(mpe.HoursPerDay), 0) AS 'VacationHours'			
    FROM    dbo.Milestone AS m
            INNER JOIN dbo.MilestonePerson AS mp ON m.MilestoneId = mp.MilestoneId
            INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
            INNER JOIN dbo.v_PersonCalendar AS cal ON cal.Date BETWEEN mpe.StartDate
                                                                 AND     ISNULL(mpe.EndDate, m.ProjectedDeliveryDate)
                                                        AND cal.PersonId = mp.PersonId
            WHERE cal.DayOff = 1 AND 
				(DATEPART(weekday, cal.Date) != 1 AND DATEPART(weekday, cal.Date) != 7)
	GROUP BY m.MilestoneId,
			mp.MilestonePersonId,
			mp.PersonId,
			mpe.StartDate,
			mpe.EndDate
--    ORDER BY m.MilestoneId, mp.MilestonePersonId, mp.PersonId

