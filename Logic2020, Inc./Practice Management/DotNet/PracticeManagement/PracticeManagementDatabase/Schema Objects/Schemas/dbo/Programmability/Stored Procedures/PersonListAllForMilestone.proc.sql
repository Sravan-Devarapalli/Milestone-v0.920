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

	DECLARE @FutureDate DATETIME

	SELECT @FutureDate = dbo.GetFutureDate()

	SELECT p.PersonId,
		   p.FirstName,
		   p.LastName,
		   p.IsDefaultManager,
		   p.HireDate,
		  p.IsStrawman AS IsStrawman
	  FROM dbo.Person AS p
	 WHERE 
	  (p.PersonStatusId IN (1,3,5)
	        OR EXISTS (SELECT 1
	                     FROM dbo.MilestonePerson AS mp
	                    WHERE mp.MilestonePersonId = @MilestonePersonId AND mp.PersonId = p.PersonId))
	   AND (   @StartDate IS NULL
	        OR @EndDate IS NULL
	        OR (@StartDate <= ISNULL(p.TerminationDate,@FutureDate) AND p.HireDate <= @EndDate)
	       )	  
	ORDER BY p.LastName, p.FirstName

