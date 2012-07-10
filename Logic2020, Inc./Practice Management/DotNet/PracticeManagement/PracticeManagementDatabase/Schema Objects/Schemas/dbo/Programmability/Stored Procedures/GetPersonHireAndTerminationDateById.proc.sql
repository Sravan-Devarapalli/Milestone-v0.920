-- =============================================
-- Author:		Shwetha
-- Create date: 10-07-2012
-- Last Updated by:	
-- Last Update date:
-- Read Hire Date and Termination Date for a person.	
-- =============================================
CREATE PROCEDURE [dbo].[GetPersonHireAndTerminationDateById]
(
	@PersonId	INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT p.HireDate,
	       p.TerminationDate
	  FROM dbo.Person AS p
	 WHERE p.PersonId = @PersonId
END

GO

