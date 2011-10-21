﻿CREATE PROCEDURE dbo.PersonOneOffList
(
	@DateToday   DATETIME,
	@MaxSeniorityLevel	INT
)
AS
	SET NOCOUNT ON

	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
	       p.PTODaysPerAnnum,
	       p.HireDate,
	       p.TerminationDate,
	       p.Alias,
	       p.DefaultPractice,
	       p.PracticeName,
	       p.PersonStatusId,
	       p.PersonStatusName,
		   p.EmployeeNumber,
		   p.SeniorityId,
		   p.SeniorityName,
	       p.ManagerId,
	       p.ManagerAlias,
	       p.ManagerFirstName,
	       p.ManagerLastName,
	       p.PracticeOwnedId,
	       p.PracticeOwnedName, 
	       p.TelephoneNumber
	  FROM dbo.v_Person AS p
      LEFT JOIN dbo.Practice AS pr ON p.DefaultPractice = pr.PracticeId
	 WHERE p.PersonStatusId in (1,2,3) AND ISNULL(pr.IsCompanyInternal, 0) = 0
           AND ((@MaxSeniorityLevel IS NULL) OR (@MaxSeniorityLevel < p.SeniorityValue))
		   AND EXISTS (SELECT 1 FROM dbo.v_Pay y
						WHERE p.PersonId = y.PersonId 
						AND  (@DateToday <= y.StartDate
						 OR @DateToday BETWEEN y.StartDate AND (ISNULL(y.EndDate, dbo.GetFutureDate()) - 1)
						 )
					  )
     ORDER BY p.LastName

