-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-13-2008
-- Description:	Lists overheads by the persons.
-- =============================================
CREATE VIEW [dbo].[v_PersonOverhead]
AS
	-- Fixed rates part
	SELECT ps.PersonId,
	       r.Description,
	       r.Rate,
	       r.HoursToCollect,
	       r.StartDate,
	       r.EndDate,
	       CAST(r.IsPercentage AS BIT) AS IsPercentage,
	       r.OverheadFixedRateId,
	       r.OverheadRateTypeId,
	       r.OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       CROSS JOIN dbo.v_OverheadFixedRate AS r
	 WHERE EXISTS (SELECT 1
	                 FROM dbo.Pay AS p
	                      INNER JOIN dbo.OverheadFixedRateTimescale AS rt ON p.Timescale = rt.TimescaleId
	                WHERE p.Person = ps.PersonId
	                  AND rt.OverheadFixedRateId = r.OverheadFixedRateId
	                  AND p.StartDate <= GETDATE()
	                  AND p.EndDate > GETDATE())
	   AND r.OverheadRateTypeId NOT IN (2, 4)
	   AND r.[Inactive] = 0
	   AND r.[StartDate] <= GETDATE()
	   AND ISNULL(r.[EndDate], dbo.GetFutureDate()) > GETDATE()
	   AND r.IsCogs = 1
	UNION ALL
	-- Dynamically calculated rates
	-- Pay rate multiplier
	SELECT ps.PersonId,
	       r.Description,
	       r.Rate * p.[Amount] / 100 AS Rate,
	       CAST(CASE p.[Timescale] WHEN 2 THEN dbo.GetHoursPerYear() ELSE 1 END AS INT) AS HoursToCollect,
	       r.StartDate,
	       r.EndDate,
	       CAST(0 AS BIT) AS IsPercentage,
	       r.OverheadFixedRateId,
	       r.OverheadRateTypeId,
	       r.OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       CROSS JOIN dbo.v_OverheadFixedRate AS r
	       INNER JOIN dbo.Pay AS p
	           ON ps.[PersonId] = p.[Person] AND p.StartDate <= GETDATE() AND p.EndDate > GETDATE()
	       INNER JOIN dbo.OverheadFixedRateTimescale AS t
	           ON t.OverheadFixedRateId = r.OverheadFixedRateId AND t.TimescaleId = p.Timescale
	 WHERE r.[OverheadRateTypeId] = 4
	   AND r.[Inactive] = 0
	   AND p.[StartDate] <= GETDATE()
	   AND p.[EndDate] > GETDATE()
	   AND r.[StartDate] <= GETDATE()
	   AND ISNULL(r.[EndDate], dbo.GetFutureDate()) > GETDATE()
	   AND r.IsCogs = 1
	UNION ALL
	-- Bill rate multiplier
	SELECT ps.PersonId,
	       r.Description,
	       ISNULL((SELECT r.Rate * SUM(mp.MilestoneHourlyRevenue) / 100
	                 FROM dbo.v_MilestonePersonLight AS mp
	                WHERE mp.PersonId = ps.PersonId
	                  AND mp.StartDate <= GETDATE()
	                  AND mp.EndDate > GETDATE()), 0) AS Rate,
	       CAST(CASE p.[Timescale] WHEN 2 THEN dbo.GetHoursPerYear() ELSE 1 END AS INT) AS HoursToCollect,
	       r.StartDate,
	       r.EndDate,
	       CAST(0 AS BIT) AS IsPercentage,
	       r.OverheadFixedRateId,
	       r.OverheadRateTypeId,
	       r.OverheadRateTypeName,
	       r.Rate AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       CROSS JOIN dbo.v_OverheadFixedRate AS r
	       INNER JOIN dbo.Pay AS p
	           ON ps.[PersonId] = p.[Person] AND p.StartDate <= GETDATE() AND p.EndDate > GETDATE()
	       INNER JOIN dbo.OverheadFixedRateTimescale AS t
	           ON t.OverheadFixedRateId = r.OverheadFixedRateId AND t.TimescaleId = p.Timescale
	 WHERE r.[OverheadRateTypeId] = 2
	   AND r.[Inactive] = 0
	   AND p.[StartDate] <= GETDATE()
	   AND p.[EndDate] > GETDATE()
	   AND r.[StartDate] <= GETDATE()
	   AND ISNULL(r.[EndDate], dbo.GetFutureDate()) > GETDATE()
	   AND r.IsCogs = 1
	UNION ALL
	-- Recruiting overhead
	SELECT ps.[PersonId],
	       'Recruiting' AS [Description],
	       ROUND(SUM(r.[Amount] / r.[HoursToCollect]), 2) AS Rate,
	       1 AS HoursToCollect,
	       ps.[HireDate] AS [StartDate],
	       MAX(ps.[TerminationDate]) AS [EndDate],
	       CAST(0 AS BIT) AS IsPercentage,
	       NULL AS OverheadFixedRateId,
	       NULL AS OverheadRateTypeId,
	       NULL AS OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.[RecruiterCommission] AS r
	       INNER JOIN dbo.Person AS ps ON r.[RecruitId] = ps.[PersonId]
	 WHERE (SELECT COUNT(*) * 8
			  FROM dbo.Calendar AS cal
			 WHERE cal.Date BETWEEN ps.[HireDate] AND GETDATE()
			   AND cal.DayOff = 0) < r.[HoursToCollect]
	GROUP BY ps.[PersonId], ps.[HireDate]
	UNION ALL
	SELECT ps.[PersonId],
	       'Bonus' AS [Description],
	       p.BonusAmount AS Rate,
	       p.BonusHoursToCollect,
	       p.StartDate,
	       p.EndDate,
	       CAST(0 AS BIT) AS IsPercentage,
	       NULL AS OverheadFixedRateId,
	       NULL AS OverheadRateTypeId,
	       NULL AS OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       INNER JOIN dbo.Pay AS p ON ps.[PersonId] = p.[Person]
	 WHERE p.[StartDate] <= GETDATE()
	   AND p.[EndDate] > GETDATE()

