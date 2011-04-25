-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-17-2008
-- Updated by:  Anatoliy Lokshin
-- Update date: 10-22-2008
-- Updated by: Nikita Goncharenko
-- Update date: 11-02-2009
-- Description:	List all persons that have some bench time
-- =============================================
CREATE PROCEDURE dbo.PersonListBenchExpense_fast
(
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON

	-- Listing all records
	SELECT p.PersonId, p.LastName, p.FirstName,
	       p.HireDate, ISNULL(p.TerminationDate, dbo.GetFutureDate()) AS TerminationDate,
	       dbo.MakeDate(p.Year, p.Month, 1) AS Month,
		   p.Revenue,
		   ISNULL(p.SalaryCogs, 0)/* + ISNULL(p.HourlyCogs, 0)*/ AS Cogs,
	       p.Revenue - (ISNULL(p.SalaryCogs, 0) + ISNULL(p.HourlyCogs, 0)) AS Margin,
            p.Overheads,
            p.Salary,
            p.Vacations,
	       p.PersonStatusId,
	       p.SeniorityId,
	       PersonStatusName = (SELECT Name FROM dbo.PersonStatus AS ps WHERE ps.PersonStatusId = p.PersonStatusId),
	       BenchStartDate =
	           (SELECT MIN(pcal.Date)
	              FROM dbo.PersonCalendarAuto AS pcal
	             WHERE pcal.PersonId = p.PersonId
	               AND pcal.Date BETWEEN p.MonthStart AND p.MonthEnd
	               AND pcal.DayOff = 0),
	       CASE
	           -- The person is busy during the month
	           WHEN p.FirstFreeDay IS NULL AND p.AvailableFrom IS NOT NULL THEN dbo.GetFutureDate()
	           -- The person is free during the month
	           WHEN p.AvailableFrom IS NULL THEN NULL
	           -- The person becomes free within the month
	           ELSE p.FirstFreeDay
	       END AS AvailableFrom,
	       CASE WHEN HourlyCogs IS NULL OR HourlyCogs = 0 THEN 2 ELSE 1 END AS Timescale
	  FROM (
			SELECT p.PersonId, p.LastName, p.FirstName, p.HireDate, p.TerminationDate,
				   DATEPART(yyyy, cal.Date) AS Year, DATEPART(mm, cal.Date) AS Month,

				   CAST(ISNULL(SUM(
	                   CASE 
	                       WHEN mp.ProjectStatusId IN (3,2) -- Active & Projected projects
	                       THEN ms.HoursPerDay * mp.MilestoneHourlyRevenue
	                       ELSE 0
	                   END), 0) AS DECIMAL(18,2)) AS Revenue,

				   /* Compute an expense for a month for the salaried persons*/
	               -- Overheads
				   (SELECT CAST(ISNULL(SUM(
							   CASE o.IsPercentage
							       WHEN 0
							       THEN o.HourlyRate * 8 * 20 / ([dbo].[GetDaysInMonth](cal1.Date))
							       ELSE (pay.Amount * o.HourlyRate * 8 * 20) / (dbo.GetHoursPerYear() * 100 * [dbo].[GetDaysInMonth](cal1.Date))
							   END), 0) AS DECIMAL(18,2))
	               -- Salary
	                        + dbo.ComputeMonthSalary(MAX(pay.Amount), p.HireDate, p.TerminationDate, MIN(cal1.Date))
	               -- Vacations
	                        + MAX( (pay.Amount / dbo.GetHoursPerYear()) * pay.DefaultHoursPerDay * ISNULL(pay.VacationDays, 0) * 8 * 20 / (dbo.GetHoursPerYear()))
					  FROM dbo.Calendar AS cal1
						   INNER JOIN dbo.Pay AS pay
							   ON p.PersonId = pay.Person AND pay.StartDate <= cal1.Date AND pay.EndDate > cal1.Date
						   LEFT JOIN dbo.v_PersonOverheadRetrospective AS o
							   ON p.PersonId = o.PersonId AND o.Date = cal1.Date
					 WHERE cal1.Date BETWEEN MIN(cal.Date) AND MAX(cal.Date)
					   AND pay.Timescale = 2) AS SalaryCogs,
                        ( SELECT    MAX(( pay.Amount / dbo.GetHoursPerYear() )
                                          * pay.DefaultHoursPerDay
                                          * ISNULL(pay.VacationDays, 0) * 8
                                          * 20 / ( dbo.GetHoursPerYear() ))
                          FROM      dbo.Calendar AS cal1
                                    INNER JOIN dbo.Pay AS pay ON p.PersonId = pay.Person
                                                                 AND pay.StartDate <= cal1.Date
                                                                 AND pay.EndDate > cal1.Date
                                    LEFT JOIN dbo.v_PersonOverheadRetrospective
                                    AS o ON p.PersonId = o.PersonId
                                            AND o.Date = cal1.Date
                          WHERE     cal1.Date BETWEEN MIN(cal.Date) AND MAX(cal.Date)
                                    AND pay.Timescale = 2
                        ) AS Vacations,
                        ( SELECT    CAST(ISNULL(SUM(CASE o.IsPercentage
                                                      WHEN 0
                                                      THEN o.HourlyRate * 8
                                                           * 20
                                                           / ( [dbo].[GetDaysInMonth](cal1.Date) )
                                                      ELSE ( pay.Amount
                                                             * o.HourlyRate
                                                             * 8 * 20 )
                                                           / ( dbo.GetHoursPerYear() * 100 * [dbo].[GetDaysInMonth](cal1.Date) )
                                                    END), 0) AS DECIMAL(18, 2))
                          FROM      dbo.Calendar AS cal1
                                    INNER JOIN dbo.Pay AS pay ON p.PersonId = pay.Person
                                                                 AND pay.StartDate <= cal1.Date
                                                                 AND pay.EndDate > cal1.Date
                                    LEFT JOIN dbo.v_PersonOverheadRetrospective
                                    AS o ON p.PersonId = o.PersonId
                                            AND o.Date = cal1.Date
                          WHERE     cal1.Date BETWEEN MIN(cal.Date) AND MAX(cal.Date)
                                    AND pay.Timescale = 2
                        ) AS Overheads,
                        ( SELECT    dbo.ComputeMonthSalary(MAX(pay.Amount),
                                                             p.HireDate,
                                                             p.TerminationDate,
                                                             MIN(cal1.Date))
                          FROM      dbo.Calendar AS cal1
                                    INNER JOIN dbo.Pay AS pay ON p.PersonId = pay.Person
                                                                 AND pay.StartDate <= cal1.Date
                                                                 AND pay.EndDate > cal1.Date
                                    LEFT JOIN dbo.v_PersonOverheadRetrospective
                                    AS o ON p.PersonId = o.PersonId
                                            AND o.Date = cal1.Date
                          WHERE     cal1.Date BETWEEN MIN(cal.Date) AND MAX(cal.Date)
                                    AND pay.Timescale = 2
                        ) AS Salary,

	               /* Compute an expense for a month for the hourly billed persons*/
	               -- TODO: compute the vacations expense
	               (SELECT SUM(DailyRate)
	                  FROM dbo.v_MilestonePersonExpense AS e
	                 WHERE e.PersonId = p.PersonId
	                   AND e.Date BETWEEN MIN(cal.Date) AND MAX(cal.Date)
	                   AND e.Timescale IN (1, 3, 4)
	               ) AS HourlyCogs,

	               p.PersonStatusId,
	               p.SeniorityId,
	               MIN(cal.Date) AS MonthStart,
	               MAX(cal.Date) AS MonthEnd,
	               MAX(ms.Date) AS AvailableFrom,
	               MIN(CASE WHEN ms.Date IS NULL AND pcal.DayOff = 0 THEN pcal.Date ELSE NULL END) AS FirstFreeDay,
	               MAX(pcal.Date) AS PersonMonthEnd
			  FROM dbo.Person AS p
	               INNER JOIN dbo.Calendar AS cal ON cal.Date BETWEEN p.HireDate AND ISNULL(p.TerminationDate, dbo.GetFutureDate())
	               LEFT JOIN dbo.v_MilestonePersonSchedule AS ms
	                   ON ms.PersonId = p.PersonId AND ms.Date = cal.Date
	               LEFT JOIN dbo.v_MilestonePerson AS mp
	                   ON mp.PersonId = p.PersonId AND ms.MilestoneId = mp.MilestoneId AND mp.StartDate = ms.EntryStartDate
	               LEFT JOIN dbo.PersonCalendarAuto AS pcal
	                   ON pcal.Date = cal.Date AND pcal.PersonId = p.PersonId AND pcal.DayOff = 0
			 WHERE p.DefaultPractice <> 4
			   AND cal.Date BETWEEN @StartDate AND @EndDate
			   AND p.PersonStatusId != 4
			GROUP BY p.PersonId, p.LastName, p.FirstName, p.HireDate, p.TerminationDate, DATEPART(yyyy, cal.Date), DATEPART(mm, cal.Date), p.PersonStatusId, p.SeniorityId
			) AS p
	 WHERE 
		Revenue < (ISNULL(p.SalaryCogs, 0) + ISNULL(p.HourlyCogs, 0)) AND 
		p.PersonStatusId != 3 AND p.PersonStatusId != 4 AND
		dbo.GetCurrentPayType(p.PersonId) != 3 AND dbo.GetCurrentPayType(p.PersonId) != 4
	ORDER BY p.PersonId, p.LastName, p.FirstName, p.Year, p.Month

