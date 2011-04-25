-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-29-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	11-20-2008
-- Description:	Selects all subordinated persons for a specified practice manager.
-- =============================================
CREATE PROCEDURE dbo.PersonListSubordinates
(
	@PracticeManagerId   INT
)
AS
	SET NOCOUNT ON

	DECLARE @PracticeManagerSeniorityId INT

	--Get the SeniorityId of the Practice Manager and store it in a variable
	SELECT @PracticeManagerSeniorityId = SeniorityId
	FROM dbo.Person
	WHERE PersonId = @PracticeManagerId

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
	 WHERE ISNULL(pr.IsCompanyInternal, 0) = 0
					AND p.SeniorityId > @PracticeManagerSeniorityId -- All the persons, having seniority level below the Practice Manager's seniority level
					AND @PracticeManagerSeniorityId <= 65 -- According to 2656, Managers and up should be able to see their subordinates, but not equals.
	ORDER BY p.LastName, p.FirstName

