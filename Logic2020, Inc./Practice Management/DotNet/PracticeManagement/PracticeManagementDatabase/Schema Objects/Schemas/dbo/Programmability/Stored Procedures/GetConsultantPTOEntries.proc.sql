CREATE PROCEDURE [dbo].[GetConsultantPTOEntries]
(
	 @StartDate DATETIME
	,@EndDate DATETIME
	,@ActivePersons BIT = 1
	,@ProjectedPersons BIT = 1
	,@W2SalaryPersons BIT = 1
	,@W2HourlyPersons BIT = 1
	,@PracticeIds NVARCHAR(4000) = NULL
	,@DivisionIds NVARCHAR(4000) = NULL
	,@TitleIds NVARCHAR(4000) = NULL
	,@SortId INT = 1
	,@SortDirection NVARCHAR(15) = 'DESC'
)
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @PTOTimeType INT,
			@W2SalaryTimeScaleId INT = 0,
			@W2HourlyTimeScaleId INT = 0,
			@EndDateReport DATETIME,
		    @StartDateReport DATETIME

	 SET @StartDateReport = @StartDate
	 SET @EndDateReport = @EndDate

	SELECT @PTOTimeType= TimeTypeId FROM dbo.TimeType WHERE Code = 'W9310'
	IF(@W2SalaryPersons = 1)
	BEGIN
		SELECT @W2SalaryTimeScaleId= TimeScaleId FROm dbo.Timescale WHERE TimescaleCode='SLRY'
	END

	IF(@W2HourlyPersons = 1)
	BEGIN
		SELECT @W2HourlyTimeScaleId= TimeScaleId FROm dbo.Timescale WHERE TimescaleCode='HRLY'
	END


		DECLARE @CurrentConsultants TABLE (ConsId INT,TimeScaleId INT,TimeScaleName NVARCHAR(50));
        INSERT INTO @CurrentConsultants(ConsId,TimeScaleId,TimeScaleName)
        SELECT p.PersonId,T.TimescaleId,T.Name 
		FROM dbo.Person AS p
		INNER JOIN dbo.Timescale T ON T.TimescaleId IN (@W2SalaryTimeScaleId,@W2HourlyTimeScaleId)
		INNER JOIN dbo.GetLatestPayWithInTheGivenRange(@StartDateReport,@EndDateReport) AS PCPT ON PCPT.PersonId = p.PersonId AND T.TimescaleId = PCPT.Timescale  
        LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
		LEFT JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = P.TerminationReasonId
        WHERE (p.IsStrawman = 0) 
			  AND (TR.TerminationReasonId IS NULL OR TR.IsPersonWorkedRule = 1)
              AND ((@ActivePersons = 1 AND p.PersonStatusId IN (1,5)) OR (@ProjectedPersons = 1 AND p.PersonStatusId = 3))
			  AND (@PracticeIds IS NULL OR p.DefaultPractice IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PracticeIds)))
			  AND (@TitleIds IS NULL OR p.TitleId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@TitleIds)))
			  AND (@DivisionIds IS NULL OR p.DivisionId IN (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@DivisionIds)))

	  SELECT  p.PersonId
			 ,p.EmployeeNumber
			 ,p.First AS FirstName
			 ,p.LastName
			 ,p.HireDate
			 ,p.TerminationDate
			 ,c.TimescaleId
			 ,c.TimeScaleName AS Timescale
			 ,st.PersonStatusId
			 ,st.Name
			 ,P.TitleId
			 ,p.Title
			 ,p.DefaultPractice PracticeId
			 ,p.PracticeName
			 ,ISNULL(VactionDaysTable.VacationDays,0) AS PersonVactionDays
	  FROM v_person AS p 
	       INNER JOIN @CurrentConsultants AS c ON c.ConsId = p.PersonId
		   INNER JOIN dbo.PersonStatus AS st ON p.PersonStatusId = st.PersonStatusId
		   LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
           LEFT JOIN dbo.GetPersonVacationDaysTable(@StartDateReport,@EndDateReport) VactionDaysTable ON VactionDaysTable.PersonId = c.ConsId
	 ORDER BY 
			CASE WHEN  @SortDirection = 'DESC' THEN P.LastName END DESC,
			CASE WHEN  @SortDirection = 'ASC' THEN P.LastName END ASC
		

	  SELECT PH.PersonId
			,PH.HireDate
			,PH.TerminationDate 
	  FROM v_PersonHistory PH 
	       INNER JOIN @CurrentConsultants AS c ON c.ConsId = PH.PersonId
	  ORDER BY PH.PersonId,PH.HireDate

	--To get PTO Hours
	 SELECT	PC.PersonId
		   ,PC.Date
		   ,CASE WHEN PC.TimeTypeId=@PTOTimeType THEN 1 ELSE 2 END AS IsTimeOff
		   ,PC.Description
		   ,ROUND(ISNULL(PC.ActualHours,0),2) TimeOffHours
	 FROM dbo.PersonCalendar PC 
		  INNER JOIN @CurrentConsultants AS c ON c.ConsId=PC.PersonId AND PC.Date BETWEEN @StartDateReport AND @EndDateReport
		  --LEFT JOIN dbo.Calendar AS Cal ON Cal.Date=PC.Date
	 WHERE PC.DayOff=1  AND DATEPART(DW,PC.Date) NOT IN (1,7) --AND PC.TimeTypeId=@PTOTimeType
	 
	 UNION ALL
	 -- To get Company Holidays
	 SELECT	PC.PersonId
		   ,PC.Date
		   ,0 AS IsTimeOff
		   ,Cal.HolidayDescription as Description
		   ,ROUND(ISNULL(PC.TimeOffHours,0),2) TimeOffHours
	 FROM dbo.PersonCalendarAuto PC INNER JOIN @CurrentConsultants AS c ON c.ConsId=PC.PersonId AND PC.Date BETWEEN @StartDate AND @EndDate
		LEFT JOIN dbo.Calendar AS Cal ON Cal.Date=PC.Date
		WHERE PC.DayOff=1 AND PC.CompanyDayOff=1 AND DATEPART(DW,PC.Date) NOT IN (1,7) 
		ORDER BY PC.PersonId,PC.Date
END

