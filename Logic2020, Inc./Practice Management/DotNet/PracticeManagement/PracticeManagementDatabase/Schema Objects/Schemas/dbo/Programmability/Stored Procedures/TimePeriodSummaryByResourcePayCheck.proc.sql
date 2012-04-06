-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 04-03-2012
-- Updated by : Sainath.CH
-- Update Date: 04-06-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[TimePeriodSummaryByResourcePayCheck]
(
	@StartDate DATETIME,
	@EndDate   DATETIME
)
AS
BEGIN

	SET @StartDate = CONVERT(DATE,@StartDate)
	SET @EndDate = CONVERT(DATE,@EndDate)

	DECLARE @NOW DATE
	SET @NOW = dbo.GettingPMTime(GETUTCDATE())

	;WITH AssignedPersons AS
	(
	  SELECT MP.PersonId
	  FROM  dbo.MilestonePersonEntry AS MPE 
	  INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
											AND MPE.StartDate < @EndDate 
											AND @StartDate  < MPE.EndDate
	  INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	  INNER JOIN dbo.Person AS P ON MP.PersonId = P.PersonId AND p.IsStrawman = 0
	  GROUP BY MP.PersonId
	),
	PersonWithLatestPay AS
	(
		SELECT P.PersonId,
			MAX(PA.StartDate) AS RecentStartDate
		FROM  dbo.Person P
			LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
		WHERE P.IsStrawman = 0
		GROUP BY P.PersonId
	),
	PersonWithCurrentPay AS
	(
		SELECT P.PersonId
				,TS.Name AS Timescale
		FROM  dbo.Person P
			LEFT JOIN PersonWithLatestPay PLP ON P.PersonId = PLP.PersonId
			LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
								AND ( 
										(@NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate-1,dbo.GetFutureDate()))
										 OR (@NOW NOT BETWEEN PA.StartDate  AND ISNULL(PA.EndDate-1,dbo.GetFutureDate()) AND  PLP.RecentStartDate = PA.StartDate)
									)
			LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
		WHERE P.IsStrawman = 0
	)

	SELECT	1 AS BranchID,
			CASE WHEN P.DefaultPractice = 4 THEN 100 ELSE 200 END AS DeptID,
			P.PersonId,
			P.LastName,
			P.FirstName,
			P.EmployeeNumber,
			P.PaychexID,
			ISNULL(Data.TotalHours,0) AS TotalHours,
			ISNULL(Data.PTOHours,0) AS PTOHours,
			ISNULL(Data.HolidayHours,0) AS HolidayHours,
			ISNULL(Data.JuryDutyHours,0) AS JuryDutyHours,
			ISNULL(Data.BereavementHours,0) AS BereavementHours,
			ISNULL(Data.ORTHours,0) AS ORTHours,
			PCP.Timescale
	FROM  
		(
			SELECT  TE.PersonId,
					ROUND(SUM(CASE WHEN CC.TimeEntrySectionId <> 4 THEN TEH.ActualHours ELSE 0 END),2) AS TotalHours,
					ROUND(SUM(CASE WHEN TT.Code = 'W9310' THEN TEH.ActualHours ELSE 0 END),2) AS PTOHours,
					ROUND(SUM(CASE WHEN TT.Code = 'W9320' THEN TEH.ActualHours ELSE 0 END),2) AS HolidayHours,
					ROUND(SUM(CASE WHEN TT.Code = 'W9330' THEN TEH.ActualHours ELSE 0 END),2) AS JuryDutyHours,
					ROUND(SUM(CASE WHEN TT.Code = 'W9340' THEN TEH.ActualHours ELSE 0 END),2) AS BereavementHours,
					ROUND(SUM(CASE WHEN TT.Code = 'W9300' THEN TEH.ActualHours ELSE 0 END),2) AS ORTHours
					FROM dbo.TimeEntry TE
						INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
															AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
						INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId 
						INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
						INNER JOIN dbo.TimeType TT ON CC.TimeTypeId = TT.TimeTypeId
						INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
					WHERE TE.ChargeCodeDate <= ISNULL(P.TerminationDate,dbo.GetFutureDate())
					GROUP BY TE.PersonId
		) Data
		FULL JOIN AssignedPersons AP ON AP.PersonId = Data.PersonId 
		INNER JOIN dbo.Person P ON (P.PersonId = Data.PersonId OR AP.PersonId = P.PersonId) AND P.IsStrawman = 0
		INNER JOIN PersonWithCurrentPay PCP ON P.PersonId = PCP.PersonId
		ORDER BY P.LastName,P.FirstName
END

