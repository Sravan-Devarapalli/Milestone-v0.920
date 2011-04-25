-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-17-2008
-- Description:	Retrives the projected workload for the person and the period
-- =============================================
CREATE PROCEDURE dbo.ProjectedWorkloadGetByPerson
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON
	
	SELECT mh.[MilestoneId],
	       mh.[PersonId],
	       mh.[MilestonePersonMonthHours] AS [Hours],
	       mh.[Month] AS [BilledDate]
	  FROM dbo.v_MilestonePersonMonthHours AS mh
	 WHERE mh.[PersonId] = @PersonId
	   AND mh.[Month] BETWEEN @StartDate AND @EndDate
	ORDER BY mh.[PersonId], mh.[Month]

