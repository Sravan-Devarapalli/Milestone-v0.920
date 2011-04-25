-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-06-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 11-20-2008
-- Description:	Retrives the filtered list of persons.
-- =============================================
CREATE PROCEDURE dbo.PersonListAllShort
(
	@PracticeId    INT = NULL,
	@PersonStatusId INT = NULL,
	@StartDate     DATETIME = NULL,
    @EndDate       DATETIME = NULL
)
AS
	SET NOCOUNT ON

	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
		   p.IsDefaultManager
	  FROM dbo.Person AS p
	 WHERE (@PracticeId IS NULL OR p.DefaultPractice = @PracticeId)
	   AND (@PersonStatusId IS NULL OR p.PersonStatusId = @PersonStatusId)
	   AND (   @StartDate IS NULL
	        OR @EndDate IS NULL
	        OR dbo.GetOverlappingPlacementDays(p.HireDate, ISNULL(p.TerminationDate, dbo.GetFutureDate()), @StartDate, @EndDate) = 1
	       )
	ORDER BY p.LastName, p.FirstName

