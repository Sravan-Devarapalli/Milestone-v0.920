-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-12-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-09-2008
-- Description:	Retrives the projected workload for the specified milestone by the persons
-- =============================================
CREATE PROCEDURE [dbo].[ProjectedWorkloadGetByMilestone]
(
	@MilestoneId   INT
)
AS
	SET NOCOUNT ON
	
	SELECT mh.[MilestoneId],
	       mh.[PersonId],
	       mh.[MilestonePersonMonthHours] AS [Hours],
	       mh.[Month] AS [BilledDate],
	       mh.EntryStartDate
	  FROM dbo.v_MilestonePersonMonthHours AS mh
	 WHERE mh.[MilestoneId] = @MilestoneId
	ORDER BY mh.[PersonId], mh.[Month]

