CREATE VIEW dbo.v_MilestonePerson
AS
	SELECT mpe.Id AS EntryId,
		   mp.MilestonePersonId,
	       mp.MilestoneId,
	       mp.PersonId,
	       mpe.StartDate,
	       mpe.EndDate,
	       mpe.HoursPerDay,
	       mpe.Location,
	       p.FirstName,
	       p.LastName,
	       p.SeniorityId,
	       m.ProjectId,
	       m.ProjectName,
		   m.ProjectNumber,
	       m.ProjectStartDate,
	       m.ProjectEndDate,
	       m.Discount,
	       m.ClientId,
	       m.ClientName,
	       m.Description AS MilestoneName,
	       m.IsHourlyAmount,
	       m.StartDate AS MilestoneStartDate,
	       m.ProjectStatusId,
	       m.ProjectedDeliveryDate AS MilestoneProjectedDeliveryDate,
	       m.ClientIsChargeable,
	       m.ProjectIsChargeable,
	       m.MilestoneIsChargeable,
	       m.ConsultantsCanAdjust,
	       ISNULL((SELECT COUNT(*) * mpe.HoursPerDay
				FROM dbo.v_PersonCalendar AS pcal
				WHERE  (
						(pcal.CompanyDayOff = 0 AND pcal.IsFloatingHoliday = 0) 
						OR (pcal.CompanyDayOff = 1 AND pcal.DayOff =  0)
					   )
					AND pcal.Date BETWEEN mpe.StartDate AND mpe.EndDate
					AND pcal.PersonId = mp.PersonId), 0) AS ExpectedHoursWithVacationDays,
	       mpe.Amount,
	       CASE m.IsHourlyAmount
	           WHEN 1
	           THEN mpe.Amount
	           ELSE
	              (SELECT CAST(m.Amount /NULLIF( mh.MilestoneHours,0) AS DECIMAL(18,2))
	                 FROM dbo.v_MilestoneHours AS mh
	                WHERE mh.MilestoneId = mp.MilestoneId)
	       END AS MilestoneHourlyRevenue,
	       mpe.PersonRoleId,
	       r.Name AS RoleName,
	       m.SalesCommission,
	       m.ExpectedHours AS MilestoneExpectedHours,
		   ISNULL((SELECT COUNT(*)
				FROM dbo.v_PersonCalendar AS pcal
				WHERE pcal.DayOff = 1 AND pcal.CompanyDayOff = 0 AND pcal.IsFloatingHoliday = 0
					AND pcal.Date BETWEEN mpe.StartDate AND mpe.EndDate
					AND pcal.PersonId = mp.PersonId ),0) as VacationDays
	  FROM dbo.MilestonePerson AS mp
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
	       INNER JOIN dbo.v_Milestone AS m ON mp.MilestoneId = m.MilestoneId
	       INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId
	       LEFT JOIN dbo.PersonRole AS r ON mpe.PersonRoleId = r.PersonRoleId

