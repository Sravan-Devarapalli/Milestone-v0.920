-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-17-2008
-- Updated by:  Anatoliy Lokshin
-- Update date: 10-22-2008
-- Updated by: Nikita Goncharenko
-- Update date: 11-02-2009
-- Description:	List all persons that have some bench time
-- =============================================
CREATE PROCEDURE dbo.PersonListBenchExpense_details
    (
      @StartDate DATETIME,
      @EndDate DATETIME
    )
AS 
    SET NOCOUNT ON

	-- Listing all records
    SELECT  p.PersonId,
            p.LastName,
            p.FirstName,
            p.HireDate,
            ISNULL(p.TerminationDate, dbo.GetFutureDate()) AS TerminationDate,
            dbo.MakeDate(p.Year, p.Month, 1) AS Month,
            p.Revenue,
            ISNULL(p.SalaryCogs, 0) AS Cogs,
            p.Revenue - ISNULL(p.SalaryCogs, 0) AS Margin,
            p.Overheads,
            p.Salary,
            p.Vacations,
            2 AS Timescale
    FROM    ( SELECT    p.PersonId,
                        p.LastName,
                        p.FirstName,
                        p.HireDate,
                        p.TerminationDate,
                        DATEPART(yyyy, cal.Date) AS Year,
                        DATEPART(mm, cal.Date) AS Month,
                        SUM(CASE WHEN proj.ProjectStatusId IN ( 3, 2 ) -- Active & Projected projects
                                      THEN ( CASE WHEN cal.Date BETWEEN mpe.StartDate
                                                                AND     ISNULL(mpe.EndDate, m.ProjectedDeliveryDate)
                                                  THEN mpe.HoursPerDay
                                                  ELSE 0
                                             END )
                                      * ( CASE m.IsHourlyAmount
                                            WHEN 1 THEN mpe.Amount
                                            ELSE CAST(m.Amount
                                                 / mh.MilestoneHours AS DECIMAL(18, 2))
                                          END )
                                 ELSE 0
                            END) AS Revenue,

				   /* Compute an expense for a month for the salaried persons*/
	               -- Overheads
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
	               -- Salary
                                    + dbo.ComputeMonthSalary(MAX(pay.Amount),
                                                             p.HireDate,
                                                             p.TerminationDate,
                                                             MIN(cal1.Date))
	               -- Vacations
                                    + MAX(( pay.Amount / dbo.GetHoursPerYear() )
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
                        ) AS SalaryCogs,
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
                        p.PersonStatusId,
                        p.SeniorityId,
                        MIN(cal.Date) AS MonthStart,
                        MAX(cal.Date) AS MonthEnd
              FROM      dbo.Person AS p
                        INNER JOIN dbo.MilestonePerson AS mp ON p.PersonId = mp.PersonId
                        INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
                        INNER JOIN dbo.Milestone AS m ON m.MilestoneId = mp.MilestoneId
                        INNER JOIN dbo.Project AS proj ON m.ProjectId = proj.ProjectId
                        INNER JOIN dbo.PersonCalendarAuto AS cal 
							ON p.PersonId = cal.PersonId
							AND cal.Date BETWEEN p.HireDate AND ISNULL(p.TerminationDate, dbo.GetFutureDate())
							AND cal.DayOff = 0
                        INNER JOIN dbo.v_MilestoneHours AS mh ON mh.MilestoneId = m.MilestoneId
              WHERE     p.DefaultPractice <> 4
                        AND cal.Date BETWEEN @StartDate AND @EndDate
                        AND p.PersonStatusId != 4
                        AND p.PersonStatusId != 3                        
              GROUP BY  p.PersonId,
                        p.LastName,
                        p.FirstName,
                        p.HireDate,
                        p.TerminationDate,
                        DATEPART(yyyy, cal.Date),
                        DATEPART(mm, cal.Date),
                        p.PersonStatusId,
                        p.SeniorityId
            ) AS p
    WHERE   dbo.GetCurrentPayType(p.PersonId) = 2
            AND Revenue < ISNULL(p.SalaryCogs, 0)
    ORDER BY p.PersonId,
            p.LastName,
            p.FirstName,
            p.Year,
            p.Month

