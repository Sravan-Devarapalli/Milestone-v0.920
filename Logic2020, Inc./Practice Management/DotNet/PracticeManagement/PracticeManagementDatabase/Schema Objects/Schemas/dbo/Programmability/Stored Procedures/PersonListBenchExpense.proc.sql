-- =============================================
-- Description:	List all persons that have some bench time
-- =============================================
CREATE PROCEDURE dbo.PersonListBenchExpense
(
	@StartDate   DATETIME,
	@EndDate     DATETIME,
	@ActivePersons BIT = 1,
	@ActiveProjects BIT = 1,
	@ProjectedPersons BIT = 1,
	@ProjectedProjects BIT = 1,
	@ExperimentalProjects BIT = 1,
	@CompletedProjects BIT = 1,
	@PracticeIds NVARCHAR(4000) = NULL
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
		@PracticeIdsLocal NVARCHAR(4000) = NULL
		
	SELECT  @StartDateLocal=@StartDate,
			@EndDateLocal=@EndDate,
			@ActivePersonsLocal=@ActivePersons ,
			@ActiveProjectsLocal=@ActiveProjects,
			@ProjectedPersonsLocal=@ProjectedPersons,
			@ProjectedProjectsLocal=@ProjectedProjects,
			@ExperimentalProjectsLocal=@ExperimentalProjects,
			@CompletedProjectsLocal = @CompletedProjects,
			@PracticeIdsLocal=@PracticeIds 

	;WITH CTERNOD
	AS
	(
		SELECT  p.PersonId,
				cal.Date,
				SUM(CAST(ISNULL((ms.HoursPerDay * mp.MilestoneHourlyRevenue *(1-mp.Discount/100)), 0) AS DECIMAL(18,2))) AS RNOD
		FROM dbo.Person AS p
		INNER JOIN dbo.PersonCalendarAuto AS cal 
			ON cal.Date BETWEEN p.HireDate AND ISNULL(p.TerminationDate, dbo.GetFutureDate())
					AND P.PersonId = cal.PersonId
		LEFT JOIN dbo.Practice AS pr 
			ON p.DefaultPractice = pr.PracticeId
		LEFT JOIN dbo.v_MilestonePersonSchedule AS ms 
			ON ms.PersonId = p.PersonId AND ms.Date = cal.Date
		LEFT JOIN dbo.v_MilestonePerson AS mp 
			ON mp.PersonId = p.PersonId AND ms.MilestoneId = mp.MilestoneId AND mp.StartDate = ms.EntryStartDate
		WHERE cal.Date BETWEEN @StartDateLocal AND @EndDateLocal
			AND (p.PersonStatusId = 1 AND @ActivePersonsLocal = 1
				 OR p.PersonStatusId = 3 AND @ProjectedPersonsLocal = 1)
			AND (@PracticeIdsLocal IS NULL 
					OR p.DefaultPractice IN (SELECT ResultId 
											 FROM [dbo].[ConvertStringListIntoTable] (@PracticeIdsLocal)
											 )
				)
				AND (mp.ProjectStatusId = 2 AND @ProjectedProjectsLocal = 1
					OR mp.ProjectStatusId = 3 AND @ActiveProjectsLocal = 1
					OR mp.ProjectStatusId = 5 AND @ExperimentalProjectsLocal = 1
					OR mp.ProjectStatusId = 4 AND @CompletedProjectsLocal = 1
					)
		GROUP BY p.PersonId,cal.Date
	),
	CTEFinancials
	AS
	(
		SELECT ISNULL(DC.FractionOfMargin,0) [SalesCommissionFraction],--To do need to convert to amount as it's %,
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
		LEFT JOIN dbo.DefaultCommission AS DC ON DC.PersonId = P.PersonId AND DC.type = 1
													AND cal.Date BETWEEN DC.StartDate AND DC.EndDate
		 WHERE cal.DayOff = 0 
		 AND cal.Date BETWEEN @StartDateLocal AND @EndDateLocal
		AND (p.PersonStatusId = 1 AND @ActivePersonsLocal = 1
			 OR p.PersonStatusId = 3 AND @ProjectedPersonsLocal = 1)
		AND (@PracticeIdsLocal IS NULL 
				OR p.DefaultPractice IN (SELECT ResultId 
										 FROM [dbo].[ConvertStringListIntoTable] (@PracticeIdsLocal)
										 ))
		 GROUP BY P.PersonId,
					cal.Date, 
					pay.Timescale,
					pay.Amount,
					pay.BonusHoursToCollect,
					pay.BonusAmount,
					pay.VacationDays,
					HY.HoursInYear,
					DC.FractionOfMargin
	),
	CTEFLHRDaily
	AS
	(
		SELECT 
			--(HourlyRate+OverHeadAmount)*40*4.2 SCOGS,
			--@DefaultBillRate*40*4.2 Revenue,
			--(HourlyRate+OverHeadAmount)+((@DefaultBillRate-(HourlyRate+OverHeadAmount))* [SalesCommissionFraction]) ,--SLHR+SCPH
	
			CASE WHEN ((HourlyRate+OverHeadAmount+BonusRate+RecruitingCommissionRate+VacationRate)+
						((@DefaultBillRate-(HourlyRate+OverHeadAmount+BonusRate+RecruitingCommissionRate+VacationRate))* [SalesCommissionFraction]*0.01))
						>HourlyRate+MLFOverheadRate
				 THEN ((HourlyRate+OverHeadAmount+BonusRate+RecruitingCommissionRate+VacationRate)+
						((@DefaultBillRate-(HourlyRate+OverHeadAmount+BonusRate+RecruitingCommissionRate+VacationRate))* [SalesCommissionFraction]*0.01))
				 ELSE HourlyRate+MLFOverheadRate
				 END FLHR,
				 --CONVERT(DECIMAL(18,2),((@DefaultBillRate-(HourlyRate+OverHeadAmount))* [SalesCommissionFraction]*0.01)) SCPH,
			PersonId,
			Date,
			CASE WHEN Timescale != 2 THEN 1 ELSE 2 END Timescale
			--SalesCommissionFraction,
			--HourlyRate,
			--OverHeadAmount,
			--BonusRate,
			--RecruitingCommissionRate,
			--VacationRate,
			--MLFOverheadRate
		FROM CTEFinancials

	)

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
			MAX(FLHRD.Timescale) Timescale,
			ISNULL(SUM(R.RNOD),0) Revenue,
			ISNULL(SUM(FLHRD.FLHR*8),0) COGS,
			(ISNULL(SUM(R.RNOD),0) - ISNULL(SUM(FLHRD.FLHR*8),0)) Margin
	FROM CTEFLHRDaily FLHRD
	LEFT JOIN  CTERNOD R ON R.[Date] = FLHRD.Date AND R.PersonId = FLHRD.PersonId
	JOIN Person P ON FLHRD.PersonId = P.PersonId
	LEFT JOIN Practice pr ON P.DefaultPractice = pr.PracticeId
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
				p.SeniorityId
	HAVING ISNULL(SUM(R.RNOD),0) < 
			ISNULL(SUM(FLHRD.FLHR*8),0)   
	ORDER BY FLHRD.PersonId, p.LastName, p.FirstName, YEAR(FLHRD.Date), MONTH(FLHRD.Date)


	/*
Person Status:

PersonStatusId	Name
1	Active
2	Terminated
3	Projected
4	Inactive



Project Status:

ProjectStatusId	Name
1	Inactive
2	Projected
3	Active
4	Completed
5	Experimental
	*/

