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
	ELSE
	BEGIN
		EXEC dbo.MilestonePersonDeleteEntries @MilestonePersonId = @MilestonePersonId
		
		DELETE
		  FROM dbo.MilestonePerson
		 WHERE MilestonePersonId = @MilestonePersonId
	END

