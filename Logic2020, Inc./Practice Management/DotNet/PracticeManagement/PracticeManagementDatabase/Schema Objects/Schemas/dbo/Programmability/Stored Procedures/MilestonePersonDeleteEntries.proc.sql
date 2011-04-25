-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-09-2008
-- Updated by:	
-- Update date:	
-- Description:	Removes persons-milestones details for the specified milestone and person.
-- =============================================
CREATE PROCEDURE [dbo].[MilestonePersonDeleteEntries]
(
	@MilestonePersonId   INT
)
AS
	SET NOCOUNT ON
	SET NOCOUNT ON
		DELETE
		  FROM dbo.MilestonePersonEntry
		 WHERE MilestonePersonId = @MilestonePersonId

