-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 12-04-2008
-- Updated by:	
-- Update date: 
-- Description:	Retrives the filtered list of persons who are not in the Administration practice.
-- =============================================
CREATE PROCEDURE dbo.PersonListAllForMilestone
(
	@MilestonePersonId   INT,
	@StartDate           DATETIME,
    @EndDate             DATETIME
)
AS
	SET NOCOUNT ON

	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
		   p.IsDefaultManager,
		   p.HireDate
	  FROM dbo.Person AS p
	 WHERE (   (p.PersonStatusId = 1 OR p.PersonStatusId = 3)
	        OR EXISTS (SELECT 1
	                     FROM dbo.MilestonePerson AS mp
	                    WHERE mp.MilestonePersonId = @MilestonePersonId AND mp.PersonId = p.PersonId))
	   AND (   @StartDate IS NULL
	        OR @EndDate IS NULL
	        OR dbo.GetOverlappingPlacementDays(p.HireDate, ISNULL(p.TerminationDate, dbo.GetFutureDate()), @StartDate, @EndDate) = 1
	       )	  
	ORDER BY p.LastName, p.FirstName

