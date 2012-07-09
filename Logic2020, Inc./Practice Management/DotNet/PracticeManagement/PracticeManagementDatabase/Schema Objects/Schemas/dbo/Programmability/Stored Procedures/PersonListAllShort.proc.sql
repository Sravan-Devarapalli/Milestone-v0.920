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

	DECLARE @FutureDate DATETIME
	SELECT @FutureDate = dbo.GetFutureDate()

	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
		   p.IsDefaultManager
	  FROM dbo.Person AS p
	 WHERE 
	 p.IsStrawman = 0
	 AND  (@PracticeId IS NULL OR p.DefaultPractice = @PracticeId)
	   AND (@PersonStatusId IS NULL OR p.PersonStatusId = @PersonStatusId)
	   AND (   @StartDate IS NULL
	        OR @EndDate IS NULL
	        OR (@StartDate <= ISNULL(p.TerminationDate,@FutureDate) AND p.HireDate <= @EndDate)
	       )
	ORDER BY p.LastName, p.FirstName

