CREATE PROCEDURE [dbo].[GetCohortAssignmentsForRange]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN

		;WITH PersonDetails
		AS
		(
			SELECT P.PersonId,
				   P.EmployeeNumber AS [Employee ID],
				   P.LastName+', '+P.FirstName AS [Person Name],
				   PS.Name AS Status,
				   PH.HireDate,
				   PH.TerminationDate,
				   manager.LastName+', '+manager.FirstName AS [Career Counselor],
				   CA.Name AS [Cohort Assignment]
			FROM dbo.Person P
			INNER JOIN v_PersonHistory PH ON P.PersonId = PH.PersonId AND PH.HireDate BETWEEN @StartDate AND @EndDate
			INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = PH.PersonStatusId
			LEFT JOIN dbo.Person manager ON manager.PersonId = P.ManagerId
			LEFT JOIN dbo.CohortAssignment CA ON CA.CohortAssignmentId = p.CohortAssignmentId
		),
		PersonDetailsWithStartDate
		AS
		(
		SELECT PD.PersonId,
			   PD.HireDate,
			   MAX(p.StartDate) AS StartDate,
			   PD.[Employee ID],
			   PD.[Person Name],
			   PD.Status,
			   PD.TerminationDate,
			   PD.[Career Counselor],
			   PD.[Cohort Assignment]
		FROM dbo.Pay P 
		INNER JOIN PersonDetails PD ON PD.PersonId = P.Person
		WHERE PD.HireDate <= P.EndDate-1 AND ((PD.TerminationDate IS NULL OR PD.TerminationDate > @EndDate) AND P.StartDate <= @EndDate OR P.StartDate <= PD.TerminationDate)
		GROUP BY PD.PersonId,PD.HireDate,PD.[Employee ID],PD.[Person Name],PD.Status,PD.TerminationDate,PD.[Career Counselor],PD.[Cohort Assignment]
	)
	SELECT PDWS.[Employee ID],
		   PDWS.[Person Name],
		   PDWS.Status,
		   T.Title,
		   PDWS.HireDate,
		   PDWS.TerminationDate,
		   PDWS.[Career Counselor],
		   PDWS.[Cohort Assignment]
	FROM PersonDetailsWithStartDate PDWS
	INNER JOIN dbo.Pay pay ON pay.Person = PDWS.PersonId AND pay.StartDate = PDWS.StartDate
	LEFT JOIN dbo.Title T ON T.TitleId = pay.TitleId
	WHERE pay.Timescale = 2 --FOR W2-SALARY 
	ORDER BY PDWS.PersonId
END
