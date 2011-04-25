-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-10-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-09-2008
-- Description:	Retrives the projected workload for the specified project by the persons
-- =============================================
CREATE PROCEDURE [dbo].[ProjectedWorkloadGetByProject]
(
	@ProjectId   INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON
	
	SELECT mh.[MilestoneId],
	       mh.[PersonId],
	       mh.[MilestonePersonMonthHours] AS [Hours],
	       mh.[Month] AS [BilledDate],
	       mh.EntryStartDate
	  FROM dbo.v_MilestonePersonMonthHours AS mh
	 WHERE mh.[ProjectId] = @ProjectId AND mh.[Month] BETWEEN @StartDate AND @EndDate
	ORDER BY mh.[PersonId], mh.[Month]

