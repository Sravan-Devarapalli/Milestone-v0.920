-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-13-2008
-- Updated by:	
-- Update date:	
-- Description:	List milestones
-- =============================================
CREATE VIEW [dbo].[v_MilestoneLight]
AS
	SELECT m.MilestoneId,
	       m.ProjectId,
	       m.Amount,
	       m.StartDate,
	       m.ProjectedDeliveryDate,
	       m.IsHourlyAmount
	  FROM dbo.Milestone AS m

