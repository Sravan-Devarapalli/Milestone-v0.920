-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 11-27-2008
-- Description:	Deletes a Milestone
-- =============================================
CREATE PROCEDURE [dbo].[MilestoneDelete]
(
	@MilestoneId   INT,
	@UserLogin     NVARCHAR(255)
)
AS
	SET NOCOUNT ON
	DECLARE @ErrorMessage NVARCHAR(2048)
	IF EXISTS (SELECT TOP 1 1 FROM dbo.v_TimeUnrestrictedEntriesUnrestricted AS te WHERE te.MilestoneId = @MilestoneId)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70017)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	IF EXISTS (SELECT TOP 1 1 FROM dbo.v_ProjectExpenses AS pe WHERE pe.MilestoneId = @MilestoneId)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70018)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN

		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		DELETE FROM dbo.Note
			  WHERE TargetId = @MilestoneId 
						AND NoteTargetId = 1

		DELETE FROM dbo.Milestone
			  WHERE MilestoneId = @MilestoneId

		-- End logging session
		EXEC dbo.SessionLogUnprepare

	END

