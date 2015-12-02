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
	DECLARE @ErrorMessage NVARCHAR(2048),
			@ProjectId	  INT

	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

	SELECT @ProjectId = ProjectId
	FROM dbo.Milestone WHERE MilestoneId = @MilestoneId

	UPDATE dbo.Project
	SET CreatedDate = @Today
	WHERE ProjectId = @ProjectId

	IF EXISTS (SELECT TOP 1 1 FROM dbo.v_TimeUnrestrictedEntriesUnrestricted AS te WHERE te.MilestoneId = @MilestoneId)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70017)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	--IF EXISTS (SELECT TOP 1 1 FROM dbo.v_ProjectExpenses AS pe WHERE pe.MilestoneId = @MilestoneId)
	--BEGIN
	--	SELECT @ErrorMessage = [dbo].[GetErrorMessage](70018)
	--	RAISERROR (@ErrorMessage, 16, 1)
	--END
	--ELSE
	BEGIN
	    
		BEGIN TRY
		BEGIN TRAN  Tran_MilestoneDelete

			-- Start logging session
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

			DELETE FROM dbo.Note
				  WHERE TargetId = @MilestoneId 
							AND NoteTargetId = 1

			DELETE MPE
			FROM dbo.MilestonePersonEntry AS MPE
			INNER JOIN  dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN  dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
			WHERE M.MilestoneId = @MilestoneId
		    
			DELETE ProjectExpense
			WHERE MilestoneId = @MilestoneId

			DELETE FROM dbo.Milestone
			WHERE MilestoneId = @MilestoneId

			IF @ProjectId IS NOT NULL
			BEGIN
				EXEC [dbo].[InsertProjectFeedbackByMilestonePersonId] @MilestonePersonId=NULL,@MilestoneId = NULL,@ProjectId = @ProjectId
			END

			-- End logging session
			EXEC dbo.SessionLogUnprepare

		COMMIT TRAN Tran_MilestoneDelete
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN Tran_MilestoneDelete
			DECLARE @ErrorMessage_1 NVARCHAR(MAX)
			SET @ErrorMessage_1 = ERROR_MESSAGE()
			RAISERROR(@ErrorMessage_1, 16, 1)
	    END CATCH

	END

