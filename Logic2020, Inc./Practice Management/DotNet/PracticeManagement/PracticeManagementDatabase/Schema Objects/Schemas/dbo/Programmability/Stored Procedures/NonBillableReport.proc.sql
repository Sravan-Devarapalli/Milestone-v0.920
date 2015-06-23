CREATE PROCEDURE [dbo].[NonBillableReport]
(
   @StartDate		DATETIME,
   @EndDate			DATETIME,
   @ProjectNumber	NVARCHAR(20) =	NULL,
   @DirectorIds		NVARCHAR(MAX) = NULL,
   @BusinessUnitIds		NVARCHAR(MAX) = NULL,
   @PracticeIds		NVARCHAR(MAX) = NULL
)
AS
BEGIN

		DECLARE @StartDateLocal DATETIME ,
				@EndDateLocal DATETIME,
				@FutureDate DATETIME,
				@HolidayTimeType INT
		DECLARE @DirectorIdsTable TABLE ( Ids INT )
		DECLARE @GroupIdsTable TABLE ( Ids INT )
		DECLARE @PracticeIdsTable TABLE ( Ids INT )

		INSERT INTO @DirectorIdsTable(Ids)
		SELECT ResultId
		FROM [dbo].[ConvertStringListIntoTable](@DirectorIds)

		INSERT INTO @GroupIdsTable(Ids)
		SELECT ResultId
		FROM [dbo].[ConvertStringListIntoTable](@BusinessUnitIds)

		INSERT INTO @PracticeIdsTable(Ids)
		SELECT ResultId
		FROM [dbo].[ConvertStringListIntoTable](@PracticeIds)

		SET @StartDateLocal = CONVERT(DATE, @StartDate)
		SET @EndDateLocal = CONVERT(DATE, @EndDate)
		SET @FutureDate = dbo.GetFutureDate()
		SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()
		;WITH BillableHours 
		AS
		(
			SELECT TE.PersonId,PRO.ProjectId,TE.ChargeCodeDate,
				 ROUND(SUM(CASE
					WHEN TEH.IsChargeable = 1 AND PRO.ProjectNumber != 'P031000' THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS [BillableHours]
			  , ROUND(SUM(CASE
					WHEN TEH.IsChargeable = 0 OR PRO.ProjectNumber = 'P031000' THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS [NonBillableHours]
			FROM TimeEntry TE
			INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId
			INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
														AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
			INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = TE.PersonId
													AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
													AND ISNULL(PTSH.EndDate,@FutureDate)
			INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId 
										AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
										AND ( 
												CC.timeTypeId != @HolidayTimeType
												OR ( 
														CC.timeTypeId = @HolidayTimeType
														AND PTSH.PersonStatusId IN (1,5)
													)
											)
			INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
			WHERE  (@ProjectNumber IS NULL OR pro.ProjectNumber = @ProjectNumber)
					AND (@BusinessUnitIds IS NULL
							OR CC.ProjectGroupId IN (SELECT Ids
													 FROM @GroupIdsTable )
						)
				   AND (@DirectorIds IS NULL
							 OR PRO.ExecutiveInChargeId IN (SELECT Ids
												   FROM @DirectorIdsTable )
						)
				  AND (@PracticeIds IS NULL
							OR PRO.PracticeId IN (SELECT Ids
												  FROM @PracticeIdsTable )
					  )
				  AND PRO.IsAllowedToShow = 1
			GROUP BY TE.PersonId,
					PRO.ProjectId,
					TE.ChargeCodeDate
		),
		TotalHours
		AS
		(
		    SELECT B.PersonId,
				   B.ProjectId,
				   SUM(B.BillableHours) AS BillableHours,
				   SUM(B.NonBillableHours) AS NonBillableHours,
				   MAX(B.ChargeCodeDate) AS LatestDate
			FROM BillableHours B
			GROUP BY B.PersonId,
					 B.ProjectId
		),
		MilestoneInfo
		AS
		(	
			SELECT   MP.PersonId ,
					 Pro.ProjectId,
					 M.MilestoneId,
					 M.IsHourlyAmount,
					 MPE.StartDate,
					 MPE.EndDate,
					 CASE WHEN M.IsHourlyAmount = 1 THEN MPE.Amount
						  ELSE M.Amount END AS Amount,
					 ROW_NUMBER() OVER (PARTITION BY MP.PersonId,Pro.ProjectId ORDER BY MPE.StartDate DESC) AS RNo
			FROM	dbo.Project Pro
				INNER JOIN dbo.Milestone AS M ON M.ProjectId = Pro.ProjectId
				INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestoneId = M.MilestoneId
				INNER JOIN dbo.MilestonePersonEntry AS MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
			WHERE 
				 (@ProjectNumber IS NULL OR pro.ProjectNumber = @ProjectNumber)
				   AND (@BusinessUnitIds IS NULL
							OR Pro.GroupId IN (SELECT Ids	FROM @GroupIdsTable )
					)
				   AND (@DirectorIds IS NULL
							 OR PRO.ExecutiveInChargeId IN (SELECT Ids FROM @DirectorIdsTable )
					)
				  AND (@PracticeIds IS NULL
							OR PRO.PracticeId IN (SELECT Ids FROM @PracticeIdsTable )
					)
				  AND (MPE.StartDate <= @EndDateLocal AND @StartDateLocal <= MPE.EndDate) 
				  AND PRO.IsAllowedToShow = 1
		),
		HourlyBillRate 
		AS 
		(
			SELECT	s.ProjectId, s.MilestoneId,
					SUM(s.HoursPerDay) AS TotalHours
			FROM dbo.v_MilestonePersonSchedule AS s
			JOIN (SELECT DISTINCT MI.MilestoneId,MI.ProjectId FROM MilestoneInfo MI 
						 WHERE MI.IsHourlyAmount = 0 AND MI.RNo = 1) A ON A.MilestoneId = s.MilestoneId
			GROUP BY s.MilestoneId,s.ProjectId
		)
		,
		PersonsForFLHR
		as
		(
			SELECT	Distinct ISNULL(M.PersonId,T.PersonId) PersonId,
					CASE WHEN ISNULL(M.EndDate,T.LatestDate) < @EndDateLocal THEN ISNULL(M.EndDate,T.LatestDate) ELSE @EndDateLocal END AS LatestDate,
					dbo.GetPayTypeAtSpecificDate(ISNULL(M.PersonId,T.PersonId),CASE WHEN ISNULL(M.EndDate,T.LatestDate) < @EndDateLocal THEN ISNULL(M.EndDate,T.LatestDate) ELSE @EndDateLocal END) AS PayStartDate
			FROM MilestoneInfo M
			FULL JOIN TotalHours T ON T.ProjectId = M.ProjectId AND T.PersonId = M.PersonId
			WHERE ISNULL(M.RNo,1) = 1
		)
		,
		OVERHEADS
		AS
		(
		  SELECT DISTINCT F.*,OFT.TimeScaleId,O.*,
				 CASE WHEN P.TimeScale = 2 THEN P.Amount/(C.DaysInYear*8) ELSE P.Amount END AS HourlyAmount,
				 CASE WHEN O.RateType = 2 THEN O.Rate*120/100 
				      WHEN O.RateType = 4 THEN O.Rate*(CASE WHEN P.TimeScale = 2 THEN P.Amount/(C.DaysInYear*8) ELSE P.Amount END)/100 
					  WHEN O.RateType = 3 THEN O.Rate*12/(C.DaysInYear*8)
					  ELSE O.Rate END AS OVH
		  FROM PersonsForFLHR F
		  JOIN Pay P ON P.Person = F.PersonId AND P.StartDate = F.PayStartDate
		  JOIN OverheadFixedRateTimescale OFT ON OFT.TimeScaleId = P.TimeScale
		  JOIN OverheadFixedRate O ON O.OverheadFixedRateId = OFT.OverheadFixedRateId
		  JOIN dbo.Calendar C ON C.Date = F.LatestDate
		  WHERE F.LatestDate >= O.StartDate AND (O.EndDate IS NULL OR F.LatestDate <= O.EndDate) and o.inactive = 0
		  
		  UNION ALL
		  
		  --FOR BONUS OVERHEAD
		  SELECT distinct F.PersonId,F.LatestDate,NULL,NULL,NULL,'Bonus Overhead',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
			     CASE WHEN p.BonusHoursToCollect = 0 THEN 0 ELSE p.BonusAmount/p.BonusHoursToCollect END AS OVH
		  FROM PersonsForFLHR F
		  JOIN v_pay P ON P.PersonId = F.PersonId AND P.StartDate = F.PayStartDate

		  UNION ALL

		  --FOR VACATION OVERHEAD
		  SELECT DISTINCT F.PersonId,F.LatestDate,NULL,NULL,NULL,'Vacation Overhead',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
		  ISNULL(P.VacationDays*8*(CASE WHEN P.TimeScale = 2 THEN P.Amount/(C.DaysInYear*8) ELSE P.Amount END)/(C.DaysInYear*8),0)
		  FROM PersonsForFLHR F
		  JOIN dbo.Pay P ON P.Person = F.PersonId AND P.StartDate = F.PayStartDate
		  JOIN dbo.Calendar C ON C.Date = F.LatestDate
		 
		 UNION ALL

		 --FOR RAW OVERHEAD
		  SELECT DISTINCT F.PersonId,F.LatestDate,NULL,NULL,NULL,'Raw Overhead',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
		  CASE WHEN P.TimeScale = 4 THEN P.Amount*120/100 
		       WHEN P.TimeScale = 2 THEN P.Amount/(C.DaysInYear*8)
			   ELSE P.Amount END
		  FROM PersonsForFLHR F
		  JOIN dbo.Pay P ON P.Person = F.PersonId AND P.StartDate = F.PayStartDate
		  JOIN dbo.Calendar C ON C.Date = F.LatestDate

		  UNION ALL

		 --FOR MLF OVERHEAD
		  SELECT DISTINCT F.PersonId,F.LatestDate,NULL,NULL,NULL,'MLF',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
		  MLFH.Rate * (CASE WHEN P.TimeScale = 4 THEN P.Amount*120/100 
							WHEN P.TimeScale = 2 THEN P.Amount/(C.DaysInYear*8)
							ELSE P.Amount END)/100
		  FROM PersonsForFLHR F
		  JOIN dbo.Pay P ON P.Person = F.PersonId AND P.StartDate = F.PayStartDate
		  JOIN MinimumLoadFactorHistory MLFH ON MLFH.TimeScaleId = P.TimeScale AND (F.LatestDate >= MLFH.StartDate AND (MLFH.EndDate IS NULL OR F.LatestDate <= MLFH.EndDate))
		  JOIN dbo.Calendar C ON C.Date = F.LatestDate
		),
		FLHR
		AS
		(
		  SELECT PersonId,LatestDate,
		  CASE WHEN SUM(CASE WHEN Description = 'MLF' THEN 0 ELSE OVH END) > SUM(CASE WHEN Description = 'MLF' OR Description = 'Raw Overhead'  THEN OVH ELSE 0 END) THEN SUM(CASE WHEN Description = 'MLF' THEN 0 ELSE OVH END) 
			   ELSE SUM(CASE WHEN Description = 'MLF' OR Description = 'Raw Overhead'  THEN OVH ELSE 0 END) END AS FLHR
		  FROM OVERHEADS
		  GROUP BY PersonId,LatestDate
		)

		SELECT P.ProjectId,
			   P.Name AS ProjectName,
			   P.ProjectNumber,
			   C.ClientId,
			   C.Name AS ClientName,
			   PG.GroupId,
			   PG.Name AS GroupName,
			   per.PersonId,
			   per.LastName,
			   per.FirstName,
			   PRA.PracticeId,
			   PRA.Name AS PracticeName,
			   sales.LastName +', '+sales.FirstName  AS Salesperson,
			   director.LastName +', '+director.FirstName AS DirectorName,
			   dbo.GetProjectManagerNames(P.ProjectId) AS ProjectManagers,
			   smngr.LastName + ', '+smngr.FirstName  AS SeniorManagerName,
			   CASE WHEN ISNULL(M.EndDate,T.LatestDate) < @EndDateLocal THEN ISNULL(M.EndDate,T.LatestDate)
					ELSE @EndDateLocal END AS LatestDate,
			   CASE WHEN M.IsHourlyAmount = 1 THEN M.Amount
					ELSE M.Amount/HBR.TotalHours END AS BillRate,
			   T.BillableHours,
			   T.NonBillableHours,
			   F.FLHR,
			   P.Discount
		FROM TotalHours T
		FULL JOIN MilestoneInfo M ON M.PersonId = T.PersonId AND M.ProjectId = T.ProjectId
		LEFT JOIN HourlyBillRate HBR ON HBR.ProjectId = M.ProjectId
		LEFT JOIN FLHR F ON F.PersonId = ISNULL(M.PersonId,T.PersonId) AND F.LatestDate = (CASE WHEN ISNULL(M.EndDate,T.LatestDate) < @EndDateLocal THEN ISNULL(M.EndDate,T.LatestDate) ELSE @EndDateLocal END)
		LEFT JOIN dbo.Project P ON P.ProjectId = ISNULL(T.ProjectId,M.ProjectId)
		LEFT JOIN dbo.Client C ON C.ClientId = P.ClientId
		LEFT JOIN dbo.ProjectGroup PG ON PG.GroupId = P.GroupId
		LEFT JOIN dbo.Person per ON per.PersonId = ISNULL(T.PersonId,M.PersonId)
		LEFT JOIN dbo.Practice Pra ON Pra.PracticeId = P.PracticeId
		LEFT JOIN dbo.Person sales ON sales.PersonId = P.SalesPersonId
		LEFT JOIN dbo.Person director ON director.PersonId = P.ExecutiveInChargeId
		LEFT JOIN dbo.Person smngr ON smngr.PersonId = P.EngagementManagerId
		WHERE ISNULL(M.RNo,1) = 1
END
