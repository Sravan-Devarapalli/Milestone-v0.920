CREATE PROCEDURE dbo.PersonExcelSet
AS
	BEGIN


 IF OBJECT_ID('#LastPay') IS NOT NULL DROP TABLE #LastPay
 CREATE TABLE #LastPay
    (
      Person INT,
      EndDate SMALLDATETIME
    )
 
 INSERT INTO #LastPay
        SELECT  pay.PersonId,
                MAX(ISNULL(pay.EndDate, dbo.GetFutureDate())) AS EndDate
        FROM    v_pay AS pay
        GROUP BY pay.PersonId


    SELECT  pers.PersonId AS 'Id',
            pers.FirstName + ' ' + pers.LastName AS 'Person name',
            stat.[Name] AS 'Status',
            pers.Alias,
            pers.HireDate,
            pers.TerminationDate,
            prct.[Name] AS 'Default Practice Area',
            paytype.[Name] AS 'Pay type',
			-- if pay type is hourly (here actually, not salary)
            CASE pay.Timescale
              WHEN 5 THEN 0.0
              WHEN 2 THEN pay.Amount / 2080
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
            sen.[Name] AS 'Seniority',
            pay.VacationDays AS 'Vacation Days',
			manager.FirstName + ' ' + manager.LastName AS 'Manager Name',
            rcd.RecruiterName,
            rcd.cc1 AS 'Recruiting comission 1 $',
            rcd.cd1 AS 'Recruiting comission 1 days',
            rcd.cc2 AS 'Recruiting comission 2 $',
            rcd.cd2 AS 'Recruiting comission 2 days'
    FROM    dbo.Person AS pers
            LEFT OUTER JOIN dbo.v_Pay AS pay ON pers.PersonId = pay.PersonId
            INNER JOIN #LastPay AS lp ON lp.Person = pay.PersonId AND (lp.EndDate = pay.EndDate OR pay.EndDate IS NULL)
            LEFT OUTER JOIN dbo.Timescale AS paytype ON paytype.TimescaleId = pay.Timescale
            LEFT OUTER JOIN dbo.Seniority AS sen ON pers.SeniorityId = sen.SeniorityId
            LEFT OUTER JOIN dbo.PersonStatus AS stat ON stat.PersonStatusId = pers.PersonStatusId
            LEFT OUTER JOIN dbo.v_RecruiterComissionWithDays AS rcd ON rcd.RecruitId = pers.PersonId
            LEFT OUTER JOIN dbo.Practice AS prct ON pers.DefaultPractice = prct.PracticeId
            LEFT JOIN dbo.Person AS manager ON manager.PersonId = pers.ManagerId
    ORDER BY pers.PersonId
END

