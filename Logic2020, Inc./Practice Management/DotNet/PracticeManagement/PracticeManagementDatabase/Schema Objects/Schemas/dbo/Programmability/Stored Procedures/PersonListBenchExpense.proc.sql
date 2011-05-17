-- =============================================
-- Description:	List all persons that have some bench time
-- =============================================
CREATE PROCEDURE [dbo].[PersonListBenchExpense]
(
	@StartDate   DATETIME,
	@EndDate     DATETIME,
	@ActivePersons BIT = NULL,
	@ActiveProjects BIT = 1,
	@ProjectedPersons BIT = NULL,
	@ProjectedProjects BIT = 1,
	@ExperimentalProjects BIT = 1,
	@CompletedProjects BIT = 1,
	@PracticeIds NVARCHAR(4000) = NULL,
	@IncludeOverheads BIT = 1,
	@IncludeZeroCostEmployees BIT = 0,
	@TimeScaleIds NVARCHAR(1000) = NULL
)
AS
	SET NOCOUNT ON

	-- Listing all records
	DECLARE @DefaultBillRate INT
	SELECT @DefaultBillRate  = 120

	DECLARE @StartDateLocal   DATETIME,
		@EndDateLocal     DATETIME,
		@ActivePersonsLocal BIT,
		@ActiveProjectsLocal BIT,
		@ProjectedPersonsLocal BIT,
		@ProjectedProjectsLocal BIT,
		@ExperimentalProjectsLocal BIT,
		@CompletedProjectsLocal BIT,
		@PracticeIdsLocal NVARCHAR(4000) = NULL,
		@TimeScaleIdsLocal NVARCHAR(1000) = NULL,
		@IncludeOverheadsLocal BIT,
		@IncludeZeroCostEmployeesLocal	BIT,
		@ApplyPersonStatusFilter	BIT
		
	SELECT  @StartDateLocal=@StartDate,
			@EndDateLocal=@EndDate,
			@ActivePersonsLocal=@ActivePersons ,
			@ActiveProjectsLocal=@ActiveProjects,
			@ProjectedPersonsLocal=@ProjectedPersons,
			@ProjectedProjectsLocal=@ProjectedProjects,
			@ExperimentalProjectsLocal=@ExperimentalProjects,
			@CompletedProjectsLocal = @CompletedProjects,
			@PracticeIdsLocal=@PracticeIds,
			@TimeScaleIdsLocal = @TimeScaleIds,
			@IncludeOverheadsLocal = @IncludeOverheads,
			@IncludeZeroCostEmployeesLocal =@IncludeZeroCostEmployees 

	IF(@ActivePersonsLocal IS NULL AND  @ProjectedPersonsLocal IS NULL)
	SELECT @ApplyPersonStatusFilter = 0
	ELSE
	SELECT @ApplyPersonStatusFilter = 1

	DECLARE @DefaultMilestoneId INT
	SELECT @DefaultMilestoneId  = (SELECT  TOP 1 MilestoneId
									FROM [dbo].[DefaultMilestoneSetting])

	DECLARE @PracticeIdsTable TABLE
	(
		PracticeId INT
	)
	INSERT INTO @PracticeIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PracticeIdsLocal)
	
	DECLARE @TimeScaleIdsTable TABLE
	(
		TimeScaleId INT
	)
	INSERT INTO @TimeScaleIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@TimeScaleIdsLocal)

	;WITH  CTEFinancials
	AS
	(
		SELECT 
			   ISNULL(CASE
				   WHEN pay.Timescale IN (1, 3)
				   THEN pay.Amount
				   WHEN pay.Timescale= 4 THEN pay.Amount*@DefaultBillRate*0.01
				   ELSE pay.Amount / HY.HoursInYear
			   END,0) AS HourlyRate,
			   ISNULL(SUM(CASE OVH.[OverheadRateTypeId] WHEN 2 THEN @DefaultBillRate*OVH.[Rate]*0.01
											 WHEN 4 THEN OVH.[Rate]*(CASE
																	   WHEN pay.Timescale IN (1, 3)
																	   THEN pay.Amount
																	   WHEN pay.Timescale= 4 THEN pay.Amount*@DefaultBillRate*0.01
																	   ELSE pay.Amount / HY.HoursInYear
																   END)*0.01
											WHEN 3 THEN OVH.[Rate] * 12 / HY.HoursInYear
											ELSE  OVH.[Rate] END),0)OverHeadAmount,
										
				CASE pay.BonusHoursToCollect
				   WHEN 0 THEN 0
				   ELSE pay.BonusAmount / (CASE WHEN pay.BonusAmount = 2080 THEN HY.HoursInYear ELSE pay.BonusHoursToCollect END)
			   END AS BonusRate,
			
				ISNULL((SELECT SUM(CASE
							   WHEN DATEDIFF(DD, Person.HireDate, Calendar.Date)*8 <= rc.HoursToCollect
									AND rc.HoursToCollect > 0 
							   THEN rc.Amount / (rc.HoursToCollect)
							   ELSE NULL
						   END)
				  FROM dbo.RecruiterCommission AS rc
					   INNER JOIN dbo.Person ON Person.PersonId = rc.RecruitId
					   INNER JOIN dbo.Calendar ON Calendar.Date = cal.Date
				 WHERE rc.RecruitId = P.PersonId
			   ),0) AS RecruitingCommissionRate,
	       
				ISNULL(CASE
				   WHEN pay.Timescale IN (1, 3)
				   THEN pay.Amount
				   WHEN pay.Timescale= 4 THEN pay.Amount*@DefaultBillRate*0.01
				   ELSE pay.Amount / HY.HoursInYear
			   END   * ISNULL(pay.VacationDays,0)*8/HY.HoursInYear,0) VacationRate,
		   
			   ISNULL((SELECT 
							CASE MLFO.[OverheadRateTypeId] 
							WHEN 2 THEN @DefaultBillRate*MLFO.[Rate]*0.01
							WHEN 4 THEN MLFO.[Rate]*(CASE
														 WHEN pay.Timescale IN (1, 3) THEN pay.Amount
														 WHEN pay.Timescale= 4 THEN pay.Amount*@DefaultBillRate*0.01
														 ELSE pay.Amount / HY.HoursInYear
														 END)*0.01
							WHEN 3 THEN MLFO.[Rate] * 12 / HY.HoursInYear
							ELSE  MLFO.[Rate]   END
						FROM dbo.v_MLFOverheadFixedRateTimescale MLFO 
						WHERE MLFO.TimescaleId = pay.Timescale)
						   ,0) MLFOverheadRate,
						   P.PersonId,
						   cal.Date,
						   pay.Timescale

		FROM Person P
		INNER JOIN dbo.PersonCalendarAuto AS cal 
				ON cal.Date BETWEEN P.HireDate AND ISNULL(p.TerminationDate, dbo.GetFutureDate())
					AND P.PersonId = cal.PersonId
		JOIN  dbo.Pay ON Pay.StartDate <= cal.Date AND Pay.EndDate > cal.date AND P.PersonId = Pay.Person
		LEFT JOIN [dbo].V_WorkinHoursByYear HY ON HY.Year = YEAR(cal.Date)
		LEFT JOIN [dbo].[v_OverheadFixedRateTimescale] OVH
				ON OVH.TimescaleId = pay.Timescale AND cal.Date BETWEEN OVH.StartDate 
				AND ISNULL(OVH.EndDate, dbo.GetFutureDate()) AND OVH.Inactive = 0
		 WHERE DATEPART(DW, cal.[Date]) NOT IN(1,7)
				AND cal.Date BETWEEN @StartDateLocal AND @EndDateLocal
				AND ((p.PersonStatusId = 1 AND @ActivePersonsLocal = 1 )
					 OR (p.PersonStatusId = 3 AND @ProjectedPersonsLocal = 1 )
					 OR(( @ApplyPersonStatusFilter = 0 AND P.PersonStatusId IN(1,2))
					))
				 AND P.PersonStatusId<>4
				AND (@PracticeIdsLocal IS NULL 
					OR p.DefaultPractice IN (SELECT PracticeId FROM @PracticeIdsTable)
					)
		 GROUP BY P.PersonId,
					cal.Date, 
					pay.Timescale,
					pay.Amount,
					pay.BonusHoursToCollect,
					pay.BonusAmount,
					pay.VacationDays,
					HY.HoursInYear 
	),
	CTEFLHRDaily
	AS
	(
		SELECT 
			CASE WHEN (HourlyRate+OverHeadAmount+BonusRate+RecruitingCommissionRate+VacationRate) >HourlyRate+MLFOverheadRate
				 THEN (HourlyRate+OverHeadAmount+BonusRate+RecruitingCommissionRate+VacationRate)
				 ELSE HourlyRate+MLFOverheadRate
				 END FLHR,
			HourlyRate,
			PersonId,
			Date,
			CASE WHEN Timescale != 2 THEN 1 ELSE 2 END Timescale,
			 Timescale TimescaleId,
			MAX(Date) OVER (PARTITION BY PersonId, YEAR(Date), MONTH(Date), (CASE WHEN Timescale != 2 THEN 1 ELSE 2 END )) TimeScaleMaxDate,
			MIN(Date) OVER (PARTITION BY PersonId, YEAR(Date), MONTH(Date), (CASE WHEN Timescale != 2 THEN 1 ELSE 2 END )) TimeScaleMinDate,
			MAX(Date) OVER(PARTITION BY PersonId, YEAR(Date), MONTH(Date)) MonthMaxDate,
			MIN(Date) OVER(PARTITION BY PersonId, YEAR(Date), MONTH(Date)) MonthMinDate
		FROM CTEFinancials
	)
	,
	
	CTEFLHRAndBenchHoursDaily
	AS
	(
		SELECT  f.FLHR,
				f.HourlyRate,
				f.PersonId,
				f.Date,
				f.Timescale,
				f.TimescaleId,
				f.TimeScaleMaxDate,
				f.TimeScaleMinDate,
				f.MonthMaxDate,
				f.MonthMinDate,
				8 - SUM(ISNULL(Temp.HoursPerDay,0)) BenchHours		
		FROM CTEFLHRDaily f
		LEFT JOIN(
					SELECT MP.PersonId,
							MPE.StartDate,
							MPE.EndDate,
							MPE.HoursPerDay
					FROM MilestonePerson MP 
					JOIN Milestone M 
						ON M.MilestoneId = MP.MilestoneId AND MP.MilestoneId <> @DefaultMilestoneId
					JOIN Project proj 
						ON proj.ProjectId = M.ProjectId
								AND(proj.ProjectStatusId = 2 AND @ProjectedProjectsLocal = 1
								OR proj.ProjectStatusId = 3 AND @ActiveProjectsLocal = 1
								OR proj.ProjectStatusId = 5 AND @ExperimentalProjectsLocal = 1
								OR proj.ProjectStatusId = 4 AND @CompletedProjectsLocal = 1
								) AND proj.ProjectStatusId NOT IN(6,1)
					JOIN MilestonePersonEntry MPE 
						ON MPE.MilestonePersonId = MP.MilestonePersonId
						) Temp
		ON Temp.PersonId = f.PersonId AND f.Date BETWEEN Temp.StartDate AND Temp.EndDate 
		GROUP BY 
				f.FLHR,
				f.HourlyRate,
				f.PersonId,
				f.Date,
				f.Timescale,
				f.TimescaleId,
				f.TimeScaleMaxDate,
				f.TimeScaleMinDate,
				f.MonthMaxDate,
				f.MonthMinDate		
 
	)
	,

CTEMonthlyFinancials
AS
(
	SELECT FLHRD.PersonId,
			P.LastName,
			P.FirstName,
			ISNULL(pr.Name,'') PracticeName,
			ISNULL(pr.IsCompanyInternal,0) IsCompanyInternal,
			P.HireDate,
			ISNULL(p.TerminationDate, dbo.GetFutureDate()) AS TerminationDate,
			dbo.MakeDate(YEAR(MIN(FLHRD.Date)), MONTH(MIN(FLHRD.Date)), 1) AS [Month],
			dbo.MakeDate(YEAR(MIN(FLHRD.Date)), MONTH(MIN(FLHRD.Date)), dbo.GetDaysInMonth(MIN(FLHRD.Date))) AS MonthEnd,
			P.PersonStatusId,
			p.SeniorityId,
			PersonStatusName = (SELECT Name FROM dbo.PersonStatus AS ps WHERE ps.PersonStatusId = p.PersonStatusId),
			FLHRD.Timescale  Timescale,
			0.0 Revenue,
			0.0 COGS,
			CASE WHEN (-1*ISNULL(SUM((CASE @IncludeOverheadsLocal WHEN 1 THEN FLHRD.FLHR ELSE FLHRD.HourlyRate END) *(CASE WHEN FLHRD.BenchHours >0 Then FLHRD.BenchHours ELSE 0 END)),0))>0 THEN 0
			ELSE (-1*ISNULL(SUM((CASE @IncludeOverheadsLocal WHEN 1 THEN FLHRD.FLHR ELSE FLHRD.HourlyRate END) *(CASE WHEN FLHRD.BenchHours >0 Then FLHRD.BenchHours ELSE 0 END)),0)) END Margin,
			SUM(ISNULL(FLHRD.BenchHours,0)) MonthBenchHours,
			RANK() OVER (PARTITION BY  FLHRD.PersonId, YEAR(FLHRD.Date), MONTH(FLHRD.Date) ORDER BY (CASE WHEN Timescale != 2 THEN 1 ELSE 2 END ) DESC)  AS RowNumber,
			TimeScaleMaxDate,
			TimeScaleMinDate,
			MonthMaxDate,
			MonthMinDate
	FROM CTEFLHRAndBenchHoursDaily FLHRD
	JOIN Person P ON FLHRD.PersonId = P.PersonId
	LEFT JOIN Practice pr ON P.DefaultPractice = pr.PracticeId
	WHERE   (@TimeScaleIdsLocal IS NULL 
			OR  FLHRD.TimescaleId IN  (SELECT TimeScaleId FROM @TimeScaleIdsTable)
		)
	GROUP BY FLHRD.PersonId,
				YEAR(FLHRD.Date),
				MONTH(FLHRD.Date),
				P.LastName,
				P.FirstName,
				pr.Name ,
				pr.IsCompanyInternal,
				p.HireDate,
				p.TerminationDate,
				p.PersonStatusId,
				p.SeniorityId,
				FLHRD.Timescale,
				TimeScaleMaxDate,
				TimeScaleMinDate,
				MonthMaxDate,
				MonthMinDate
	--HAVING (SUM(ISNULL(R.BenchHours,0)) > 0 OR @IncludeZeroCostEmployeesLocal = 1)
	
)

SELECT  PersonId,
		LastName,
		FirstName,
		PracticeName,
		IsCompanyInternal,
		HireDate,
		TerminationDate,
		[Month],
		MonthEnd,
		PersonStatusId,
		SeniorityId,
		PersonStatusName,
		Timescale,
		Revenue,
		COGS,
		Margin,
		CASE WHEN TimeScaleMinDate = MonthMinDate AND  TimeScaleMaxDate = MonthMaxDate THEN  0 -- No change of time scale
			 WHEN TimeScaleMinDate = MonthMinDate AND TimeScaleMaxDate < MonthMaxDate  THEN 1  -- Salary  to $- hourly
			 WHEN TimeScaleMinDate > MonthMinDate AND TimeScaleMaxDate = MonthMaxDate THEN 2 -- $- hourly to Salary
			 ELSE 3 END -- Multiple switchins between Salary  and $- hourly
			 AS TimeScaleChangeStatus
			
FROM 
(SELECT DISTINCT MF1.*
FROM CTEMonthlyFinancials MF1
JOIN CTEMonthlyFinancials MF2
ON MF1.PersonId = MF2.PersonId  AND MF2.RowNumber = 1
		AND (MF2.MonthBenchHours > 0 OR @IncludeZeroCostEmployeesLocal = 1)
WHERE MF1.RowNumber = 1
		) Temp

 ORDER BY  PersonId, [Month]
/*
Person Status:

PersonStatusId	Name
1				Active
2				Terminated
3				Projected
4				Inactive

Project Status:

ProjectStatusId	Name
1				Inactive
2				Projected
3				Active
4				Completed
5				Experimental
	*/


