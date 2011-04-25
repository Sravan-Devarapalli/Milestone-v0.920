-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-19-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-13-2008
-- Description:	Lists overheads by the persons in the historical retrospective.
-- =============================================
CREATE VIEW [dbo].[v_PersonOverheadRetrospective]
AS
	-- Fixed rates part
	SELECT ps.PersonId,
	       r.Description,
	       r.Rate,
	       r.HoursToCollect,
	       r.IsPercentage,
	       r.OverheadFixedRateId,
	       cal.Date,
	       CASE r.HoursToCollect
	           WHEN 0
	           THEN CAST(0 AS DECIMAL)
	           ELSE r.Rate / r.HoursToCollect
	       END AS HourlyRate,
	       r.OverheadRateTypeId,
	       r.OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       CROSS JOIN dbo.v_OverheadFixedRate AS r
	       INNER JOIN dbo.Calendar AS cal
	           ON cal.Date >= r.StartDate AND cal.Date < ISNULL(r.EndDate, dbo.GetFutureDate())
	       INNER JOIN dbo.Pay AS p
	           ON p.Person = ps.PersonId AND cal.Date >= p.StartDate AND cal.Date < p.EndDate
	       INNER JOIN dbo.OverheadFixedRateTimescale AS rt
	           ON p.Timescale = rt.TimescaleId AND rt.OverheadFixedRateId = r.OverheadFixedRateId
	 WHERE r.OverheadRateTypeId NOT IN (2, 4)
	   AND r.[Inactive] = 0
	   AND r.IsCogs = 1
	UNION ALL
	-- Dynamically calculated rates
	-- Pay rate multiplier
	SELECT ps.PersonId,
	       r.Description,
	       r.Rate * p.[Amount] / 100 AS Rate,
	       CAST(CASE p.[Timescale] WHEN 2 THEN dbo.GetHoursPerYear() ELSE 1 END AS INT) AS HoursToCollect,
	       CAST(0 AS BIT) AS IsPercentage,
	       r.OverheadFixedRateId,
	       cal.Date,
	       r.Rate * p.[Amount] / (dbo.GetHoursPerYear() * 100) AS HourlyRate,
	       r.OverheadRateTypeId,
	       r.OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       CROSS JOIN dbo.v_OverheadFixedRate AS r
	       INNER JOIN dbo.Calendar AS cal
	           ON cal.Date >= r.StartDate AND cal.Date < ISNULL(r.EndDate, dbo.GetFutureDate())
	       INNER JOIN dbo.Pay AS p
	           ON p.Person = ps.PersonId AND cal.Date >= p.StartDate AND cal.Date < p.EndDate
	       INNER JOIN dbo.OverheadFixedRateTimescale AS t
	           ON t.OverheadFixedRateId = r.OverheadFixedRateId AND t.TimescaleId = p.Timescale
	 WHERE r.[OverheadRateTypeId] = 4
	   AND r.[Inactive] = 0
	   AND r.IsCogs = 1
	UNION ALL
	-- Bill rate multiplier
	SELECT ps.PersonId,
	       r.Description,
	       ISNULL((SELECT r.Rate * SUM(mp.MilestoneHourlyRevenue) / 100
	                 FROM dbo.v_MilestonePersonLight AS mp
	                WHERE mp.PersonId = ps.PersonId
	                  AND mp.StartDate <= cal.Date
	                  AND mp.EndDate > cal.Date), 0) AS Rate,
	       CAST(CASE p.[Timescale] WHEN 2 THEN dbo.GetHoursPerYear() ELSE 1 END AS INT) AS HoursToCollect,
	       CAST(0 AS BIT) AS IsPercentage,
	       r.OverheadFixedRateId,
	       cal.Date,
	       r.Rate * p.[Amount] / (dbo.GetHoursPerYear() * 100) AS HourlyRate,
	       r.OverheadRateTypeId,
	       r.OverheadRateTypeName,
	       r.Rate AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       CROSS JOIN dbo.v_OverheadFixedRate AS r
	       INNER JOIN dbo.Calendar AS cal
	           ON cal.Date >= r.StartDate AND cal.Date < ISNULL(r.EndDate, dbo.GetFutureDate())
	       INNER JOIN dbo.Pay AS p
	           ON p.Person = ps.PersonId AND cal.Date >= p.StartDate AND cal.Date < p.EndDate
	       INNER JOIN dbo.OverheadFixedRateTimescale AS t
	           ON t.OverheadFixedRateId = r.OverheadFixedRateId AND t.TimescaleId = p.Timescale
	 WHERE r.[OverheadRateTypeId] = 2
	   AND r.[Inactive] = 0
	   AND r.IsCogs = 1
	UNION ALL
	-- Recruiting overhead
	SELECT ps.[PersonId],
	       'Recruiting' AS [Description],
	       ROUND(SUM(r.[Amount] / r.[HoursToCollect]), 2) AS Rate,
	       1 AS HoursToCollect,
	       0 AS IsPercentage,
	       NULL AS OverheadFixedRateId,
	       cal.Date,
	       ROUND(SUM(r.[Amount] / r.[HoursToCollect]), 2) AS HourlyRate,
	       NULL AS OverheadRateTypeId,
	       NULL AS OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.[RecruiterCommission] AS r
	       INNER JOIN dbo.Person AS ps ON r.[RecruitId] = ps.[PersonId]
	       INNER JOIN dbo.Calendar AS cal
	           ON cal.Date >= ps.[HireDate] AND cal.Date < DATEADD(dd, HoursToCollect / 8, ps.[HireDate])
	GROUP BY ps.[PersonId], cal.Date
	UNION ALL
	SELECT ps.[PersonId],
	       'Bonus' AS [Description],
	       p.BonusAmount AS Rate,
	       p.BonusHoursToCollect,
	       0 AS IsPercentage,
	       NULL AS OverheadFixedRateId,
	       cal.Date,
	       CASE p.BonusHoursToCollect
	           WHEN 0
	           THEN CAST(0 AS DECIMAL)
	           ELSE p.BonusAmount / p.BonusHoursToCollect
	       END AS HourlyRate,
	       NULL AS OverheadRateTypeId,
	       NULL AS OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.Person AS ps
	       INNER JOIN dbo.Pay AS p ON ps.[PersonId] = p.[Person]
	       INNER JOIN dbo.Calendar AS cal
	           ON cal.Date >= p.StartDate AND cal.Date < p.EndDate

