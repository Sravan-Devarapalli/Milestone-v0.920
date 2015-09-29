CREATE PROCEDURE [dbo].[AttainmentBillableUtilizationReport]
(
	@StartDate DATETIME ,
	@EndDate DATETIME
)
AS
BEGIN
		DECLARE @StartDateLocal DATETIME ,
				@EndDateLocal DATETIME ,
				@HolidayTimeType INT ,
				@FutureDate DATETIME,
				@CurrtenDate DATETIME,
				@CurrentMonthStartDate DATETIME,
				@YTDStartDate DATETIME,
				@YTDEndDate DATETIME

			
		SELECT @StartDateLocal = CONVERT(DATE, @StartDate), 
				@EndDateLocal = CONVERT(DATE, @EndDate),
				@CurrtenDate = CONVERT(DATE,dbo.GettingPMTime(GETUTCDATE())),
				@FutureDate = dbo.GetFutureDate()
		SELECT @CurrentMonthStartDate = MonthStartDate FROM dbo.Calendar WHERE date = @CurrtenDate -1
	    SELECT @YTDStartDate =	CASE WHEN Year(DATEADD(yy,-1,@CurrtenDate)) = Year(@StartDateLocal) THEN DATEADD(yy, DATEDIFF(yy,0,@StartDateLocal), 0)
								ELSE DATEADD(d,-DATEPART(dy,@CurrtenDate)+1,@CurrtenDate) END,

			   @YTDEndDate = CASE WHEN Year(DATEADD(yy,-1,@CurrtenDate)) = Year(@StartDateLocal) THEN DATEADD(yy, DATEDIFF(yy,0,@StartDateLocal) + 1, -1) 
								ELSE DATEADD(d,-1,CONVERT(DATE,@CurrtenDate)) END
		;WITH Ranges
		AS
		(
			SELECT MonthStartDate AS StartDate,MonthEndDate AS EndDate,'M'+CONVERT(NVARCHAR,MonthNumber) AS RangeType,DATEPART(MM,MonthStartDate) AS ColOrder FROM dbo.Calendar C
			WHERE C.DATE between @StartDateLocal and @EndDateLocal
			GROUP BY C.MonthStartDate,C.MonthEndDate,C.MonthNumber 
			UNION ALL
			SELECT QuarterStartDate,QuarterEndDate,'Q'+CONVERT(NVARCHAR,DATEPART(Q,QuarterStartDate)),DATEPART(Q,QuarterStartDate)+12 FROM dbo.Calendar C
			WHERE C.DATE between @StartDateLocal and @EndDateLocal
			GROUP BY C.QuarterStartDate,C.QuarterEndDate
			UNION ALL
			SELECT @YTDStartDate,@YTDEndDate,'YTD',17
		),
		PersonsList
		AS 
		(
			SELECT P.PersonId,pay.Timescale
			FROM  dbo.Person P
			INNER JOIN dbo.Pay pay ON P.PersonId = pay.Person
			WHERE @CurrtenDate BETWEEN pay.StartDate AND ISNULL(pay.EndDate-1,@FutureDate) AND pay.Timescale IN (1,2)
					AND P.PersonStatusId IN (1,5) AND P.DivisionId IN (2,5,6) AND P.IsOffShore = 0 
		) ,
		PersonWithRanges
		AS
		(
		SELECT  pl.PersonId,R.StartDate,R.EndDate,pl.Timescale,R.RangeType,R.ColOrder
		FROM PersonsList pl
		CROSS JOIN Ranges  R
		),
		PersonRangesWithDefaultHours
		AS
		(
			SELECT  PR.Personid,
					PR.StartDate,
					PR.EndDate,
					PR.Timescale,
				    (COUNT(PCAL.Date) * 8) AS DefaultHours --Estimated working hours per day is 8.
			FROM  PersonWithRanges PR
			INNER JOIN dbo.PersonCalendarAuto PCAL ON PCAL.PersonId = PR.PersonId AND PCAL.Date BETWEEN PR.StartDate AND PR.EndDate
			WHERE DATENAME(weekday,PCAL.Date) != 'Saturday' AND DATENAME(weekday,PCAL.Date) != 'Sunday' AND PCAL.Date < @CurrtenDate
			GROUP BY PR.Personid,PR.StartDate,PR.EndDate,PR.Timescale
		),
		PersonListWithBillingHours
		AS
		(
			SELECT  PR.PersonId ,
					PR.StartDate,
					PR.EndDate,
					PR.DefaultHours,
					PR.Timescale,
					ROUND(SUM(CASE WHEN Pro.ProjectNumber != 'P031000'
								THEN TEH.ActualHours
								ELSE 0
								END), 2) AS BillableHours 
			FROM  dbo.TimeEntry TE
			INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId
			INNER JOIN PersonRangesWithDefaultHours PR ON  TE.PersonId = Pr.PersonId 
												AND TE.ChargeCodeDate BETWEEN PR.StartDate AND PR.EndDate
			INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
			INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
			WHERE     TEH.IsChargeable = 1 AND TE.ChargeCodeDate < @CurrtenDate
			GROUP BY  PR.PersonId ,
						PR.StartDate,
						PR.EndDate,
						PR.DefaultHours,PR.Timescale
		)
		SELECT PR.PersonId,ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,P.LastName,T.Title,TS.Name AS TimeScaleName,pr.StartDate,PR.EndDate,PR.RangeType,
		        CASE WHEN PR.StartDate <= @CurrentMonthStartDate
				     THEN ROUND( (ISNULL(PLb.BillableHours,0)/ISNULL(NULLIF(PRD.DefaultHours,0),1)),4) ELSE NULL END  AS BillableUtilizationPercent,
						ISNULL(PLB.BillableHours,0) AS BillableHours, 
						ISNULL(NULLIF(PRD.DefaultHours,0),0) AS AvailableHours
		FROM PersonWithRanges PR
		LEFT JOIN PersonRangesWithDefaultHours PRD ON PR.PersonId = PRD.PersonId AND PR.StartDate = PRD.StartDate AND PR.EndDate = PRD.EndDate
		LEFT JOIN PersonListWithBillingHours PLB ON PR.PersonId = PLB.PersonId AND PR.StartDate = PLB.StartDate AND PR.EndDate = PLB.EndDate
		INNER JOIN dbo.Person P ON P.PersonId = PR.PersonId
		INNER JOIN dbo.Title T ON T.TitleId = P.TitleId 
		INNER JOIN dbo.Timescale TS ON TS.TimescaleId = PR.Timescale 
		ORDER BY P.LastName,P.FirstName,PR.PersonId,PR.ColOrder
END

