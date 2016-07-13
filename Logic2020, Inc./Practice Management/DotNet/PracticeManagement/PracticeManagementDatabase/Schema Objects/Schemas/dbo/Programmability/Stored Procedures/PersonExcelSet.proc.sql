CREATE PROCEDURE dbo.PersonExcelSet
AS
BEGIN

	  DECLARE @Today DATETIME,
		 @FutureDate DATETIME 

	 SET @Today = dbo.GettingPMTime(GETUTCDATE())
	 SET @FutureDate = dbo.GetFutureDate() 

	 --As per the bug #2977 , now considering current pay instead of lastpay.
	;WITH CurrentPay AS (
		SELECT p.PersonId, ISNULL(p.EndDate, @FutureDate) AS EndDate
		FROM dbo.v_Pay AS p
		WHERE @Today >= p.StartDate
		AND @Today < ISNULL(p.EndDateOrig, @FutureDate)
	),
	CurrentElseLastPay AS--If no currentpay then considering last pay.
	(
		SELECT PersonId, EndDate
		FROM CurrentPay
		UNION
		SELECT p.PersonId, MAX(ISNULL(p.EndDate, @FutureDate)) AS EndDate
		FROM dbo.v_Pay AS p
		LEFT JOIN CurrentPay CP ON CP.PersonId = p.PersonId
		WHERE CP.PersonId IS NULL
		GROUP BY p.PersonId

		UNION
		SELECT P.PersonId, dbo.GetFutureDate()
		FROM person P
		LEFT JOIN v_pay pay on pay.PersonId = p.PersonId
		WHERE pay.PersonId IS NULL
	)
		SELECT  pers.PersonId AS 'Id',
	        pers.EmployeeNumber AS 'Employee Id',
			ISNULL(pers.PreferredFirstName,pers.FirstName) + ' ' + pers.LastName AS 'Person name',
			stat.[Name] AS 'Status',
			pers.Alias,
			pers.HireDate,
			pers.TerminationDate,
			div.DivisionName AS 'Division',
			prct.[Name] AS 'Practice Area',
			paytype.[Name] AS 'Pay type',
			-- if pay type is hourly (here actually, not salary)
			CASE pay.Timescale
				WHEN 5 THEN 0.0
				WHEN 2 THEN pay.Amount / HY.HoursInYear
				ELSE pay.Amount
			END AS 'Hourly Pay Rate',
			-- if column stores annual bonus
			CASE pay.IsYearBonus
				WHEN 1 THEN pay.BonusAmount
				ELSE 0.0
			END AS 'Annual Bonus',
			-- if column stores hourly bonus
			CASE pay.IsYearBonus
				WHEN 0 THEN pay.BonusAmount
				ELSE 0.0
			END AS 'Hourly Bonus, $',
			CASE pay.BonusHoursToCollect
				WHEN NULL THEN 0
				ELSE pay.BonusHoursToCollect
			END AS 'Hourly Bonus, hours',
			T.Title,
			pay.VacationDays AS 'PTO Accrual',
			ISNULL(manager.PreferredFirstName,manager.FirstName) + ' ' + manager.LastName AS 'Career Manager Name',
			rcd.LastName + ' ' + ISNULL(rcd.PreferredFirstName,rcd.FirstName) AS 'RecruiterName'
	FROM    dbo.Person AS pers
			INNER JOIN CurrentElseLastPay AS lp ON lp.PersonId = pers.PersonId 
			LEFT OUTER JOIN dbo.v_Pay AS pay ON pers.PersonId = pay.PersonId and (lp.EndDate = ISNULL(pay.EndDate, @FutureDate))
			LEFT OUTER JOIN dbo.Timescale AS paytype ON paytype.TimescaleId = pay.Timescale
			LEFT OUTER JOIN dbo.Title AS T ON pers.TitleId = T.TitleId
			LEFT OUTER JOIN dbo.PersonStatus AS stat ON stat.PersonStatusId = pers.PersonStatusId
			LEFT OUTER JOIN dbo.Person AS rcd ON rcd.PersonId = pers.RecruiterId
			LEFT OUTER JOIN dbo.Practice AS prct ON pers.DefaultPractice = prct.PracticeId
			LEFT OUTER JOIN dbo.PersonDivision AS div ON div.DivisionId=pers.DivisionId
			LEFT JOIN dbo.Person AS manager ON manager.PersonId = pers.ManagerId
			LEFT JOIN V_WorkinHoursByYear HY ON HY.[Year] = YEAR(@Today)
			WHERE pers.IsStrawman=0 
	ORDER BY pers.PersonId
END

