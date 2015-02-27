CREATE PROCEDURE [dbo].[GetCohortAssignmentsForRange]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN

	  ;WITH PersonHistoryCTE
		AS
		(
			SELECT DISTINCT PH.PersonId,
				   PH.HireDate,
				   PH.TerminationDate,
				   ph.TitleId,
				   ph.PersonStatusId
			FROM v_PersonHistory PH 
			INNER JOIN dbo.Pay P ON P.Person = PH.PersonId 
			WHERE (PH.HireDate BETWEEN @StartDate AND @EndDate)
			AND (PH.HireDate <= P.EndDate-1 AND ((PH.TerminationDate IS NULL OR PH.TerminationDate > @EndDate) AND P.StartDate <= @EndDate OR P.StartDate <= PH.TerminationDate))
			AND P.Timescale = 2 --FOR W2-SALARY 
		)
	   SELECT P.EmployeeNumber AS [Employee ID],
			  P.LastName+', '+P.FirstName AS [Person Name],
			  PS.Name AS Status,
			  T.Title,
			  PH.HireDate,
			  PH.TerminationDate,
			  manager.LastName+', '+manager.FirstName AS [Career Manager],
			  CA.Name AS [Cohort Assignment]
	   FROM dbo.Person P
	   INNER JOIN PersonHistoryCTE PH ON PH.PersonId = P.PersonId 
	   INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = Ph.PersonStatusId
	   LEFT JOIN dbo.Title T ON T.TitleId = PH.TitleId
	   LEFT JOIN dbo.Person manager ON manager.PersonId = P.ManagerId
	   LEFT JOIN dbo.CohortAssignment CA ON CA.CohortAssignmentId = p.CohortAssignmentId
	   ORDER BY P.PersonId
END
