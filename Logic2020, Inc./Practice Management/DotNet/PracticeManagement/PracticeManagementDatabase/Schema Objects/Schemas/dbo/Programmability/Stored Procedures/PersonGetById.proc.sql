-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-04-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 9-09-2008
-- Description:	Retrives the Person by its ID.
-- =============================================
CREATE PROCEDURE dbo.PersonGetById
(
	@PersonId	INT
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
		   p.PracticesOwned,
		   p.TelephoneNumber
	  FROM dbo.v_Person AS p
	 WHERE p.PersonId = @PersonId

