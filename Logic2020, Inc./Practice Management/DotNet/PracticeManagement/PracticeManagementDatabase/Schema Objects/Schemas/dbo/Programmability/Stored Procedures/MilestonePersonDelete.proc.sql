-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-3-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-09-2008
-- Description:	Removes persons-milestones details and association for the specified milestone and person.
-- =============================================
CREATE PROCEDURE [dbo].[MilestonePersonDelete]
(
	@MilestonePersonId   INT
)
AS
	SET NOCOUNT ON
	DECLARE @ErrorMessage NVARCHAR(2048)

	IF EXISTS (SELECT TOP 1 1 FROM dbo.TimeEntries AS te WHERE te.MilestonePersonId = @MilestonePersonId)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70019)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE IF EXISTS (SELECT 1 FROM dbo.ProjectFeedback WHERE MilestonePersonId = @MilestonePersonId AND FeedbackStatusId = 1)
	BEGIN
	    RAISERROR ('This person cannot be deleted from this milestone because project feedback has been marked as completed.  The person can be deleted from the milestone if the status of the feedback is changed to ''Not Completed'' or ''Canceled''. Please navigate to the ''Project Feedback'' tab for more information to make the necessary adjustments..', 16, 1)
	END
	ELSE
	BEGIN
	    
		--DELETE 
		--FROM dbo.ProjectFeedback 
		--WHERE MilestonePersonId = @MilestonePersonId

		EXEC dbo.MilestonePersonDeleteEntries @MilestonePersonId = @MilestonePersonId
		
		DELETE
		  FROM dbo.MilestonePerson
		 WHERE MilestonePersonId = @MilestonePersonId
	END

