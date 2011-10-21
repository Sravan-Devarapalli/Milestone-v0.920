﻿CREATE PROCEDURE dbo.GetCareerCounselorHierarchiPersons
(
	@CounselorId   INT
)
AS
	SET NOCOUNT ON

	;WITH CounselorHierarchi AS--Line Manager replaced by CareerCounselor as per #2952.
	(
		SELECT P.PersonId, P.ManagerId
		FROM Person P
		WHERE P.ManagerId = @CounselorId
		UNION ALL
		SELECT P.PersonId, P.ManagerId
		FROM Person P
		JOIN CounselorHierarchi CC ON CC.PersonId = P.ManagerId
	)

	SELECT DISTINCT p.PersonId,
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
	FROM CounselorHierarchi CC
	JOIN dbo.v_Person AS P ON P.PersonId = CC.PersonId
	ORDER BY P.LastName, P.FirstName
