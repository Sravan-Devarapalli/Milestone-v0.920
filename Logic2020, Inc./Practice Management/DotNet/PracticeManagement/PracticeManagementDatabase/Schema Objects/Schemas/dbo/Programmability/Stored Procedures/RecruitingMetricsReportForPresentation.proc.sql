CREATE PROCEDURE [dbo].[RecruitingMetricsReportForPresentation]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN

	DECLARE @Today DATE
	SELECT @Today = dbo.GettingPMTime(GETUTCDATE());
	
	WITH PersonPayToday
	AS 
	(
	SELECT p.Person,p.StartDate
	FROM dbo.Pay AS p
	INNER JOIN dbo.Person per on p.person = per.personid
	WHERE @Today BETWEEN p.StartDate AND p.EndDate
	AND per.isStrawman = 0
	),
	PersonPayBeforeToday
	AS 
	(
	SELECT p.Person, MAX(p.StartDate) AS StartDate
	FROM dbo.Pay AS p
	LEFT JOIN PersonPayToday PPT ON p.Person = PPT.Person
	WHERE PPT.Person IS NULL
		AND @Today > p.StartDate
	GROUP BY p.Person
	),
	PayTypes
	AS 
	(
	SELECT P.Person AS PersonId ,P.Timescale
	FROM dbo.Pay P
	LEFT JOIN PersonPayToday PPT ON P.Person = PPT.Person AND P.StartDate = PPT.StartDate
	LEFT JOIN PersonPayBeforeToday PPAT ON P.Person = PPAT.Person AND P.StartDate = PPAT.StartDate
	WHERE PPT.Person IS NOT NULL OR PPAT.Person IS NOT NULL
	)
	SELECT	p.EmployeeNumber AS EmployeeID,
			P.LastName+', '+P.FirstName AS [Person Name],
			PS.Name AS [Status],
			T.Title,
			p.HireDate AS [Hire Date],
			p.TerminationDate AS [Termination Date],
			TR.TerminationReason AS [Termination Reason],
			CASE WHEN p.HireDate > @Today THEN 0 WHEN p.PersonStatusId = 2 AND p.HireDate <> p.TerminationDate THEN DATEDIFF(DAY, p.HireDate, p.TerminationDate)+1 WHEN p.PersonStatusId = 2 AND p.HireDate = p.TerminationDate THEN 0 ELSE DATEDIFF(DAY, p.HireDate, @EndDate)+1 END AS LengthOfTenureInDays,
			TS.Name AS [Pay Type],
			recr.LastName+', '+recr.FirstName AS [Recruiter Name],
			JSS.Name AS [Passive/Active Candidate],
			targetRM.Name AS [Targeted Company],
			sourceRM.Name AS [Recruiting Source],
			empRef.LastName+', '+empRef.FirstName AS [Employee Referral]
	FROM dbo.Person p
	INNER JOIN dbo.PersonStatus PS ON p.PersonStatusId = PS.PersonStatusId
	INNER JOIN dbo.Title T ON T.TitleId = p.TitleId
	INNER JOIN PayTypes pay ON pay.PersonId = p.PersonId
	INNER JOIN dbo.Timescale TS ON TS.TimescaleId = pay.Timescale
	LEFT JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = p.TerminationReasonId
	LEFT JOIN dbo.Person recr ON recr.PersonId = p.RecruiterId
	LEFT JOIN dbo.JobSeekerStatus JSS ON JSS.JobSeekerStatusId = p.JobSeekerStatusId
	LEFT JOIN dbo.RecruitingMetrics sourceRM ON sourceRM.RecruitingMetricsId = p.SourceId
	LEFT JOIN dbo.RecruitingMetrics targetRM ON targetRM.RecruitingMetricsId = P.TargetedCompanyId
	LEFT JOIN dbo.Person empRef ON empRef.PersonId = p.EmployeeReferralId 
	WHERE p.HireDate BETWEEN @StartDate AND @EndDate

END
