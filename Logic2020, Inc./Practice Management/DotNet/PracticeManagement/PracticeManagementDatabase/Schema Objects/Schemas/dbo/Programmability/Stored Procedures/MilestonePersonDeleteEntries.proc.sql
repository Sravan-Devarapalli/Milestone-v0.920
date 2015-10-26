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

	BEGIN TRY
	BEGIN TRAN  Tran_MilestonePersonDelete

	DELETE
	FROM dbo.MilestonePersonEntry
	WHERE MilestonePersonId = @MilestonePersonId

	IF @MilestonePersonId IS NOT NULL
	BEGIN
		EXEC [dbo].[InsertProjectFeedbackByMilestonePersonId] @MilestonePersonId=@MilestonePersonId,@MilestoneId = NULL
	END
	
	COMMIT TRAN Tran_MilestonePersonDelete
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN Tran_MilestonePersonDelete
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

