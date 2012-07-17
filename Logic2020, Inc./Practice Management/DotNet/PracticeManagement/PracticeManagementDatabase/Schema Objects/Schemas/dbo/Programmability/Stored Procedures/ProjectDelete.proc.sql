﻿CREATE PROCEDURE [dbo].[ProjectDelete]
(
	@ProjectID INT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN
	DECLARE @ErrorMessage NVARCHAR(MAX)
		
	IF NOT EXISTS (SELECT 1 FROM dbo.Project WHERE ProjectId = @ProjectID AND ProjectStatusId IN (1, 5))
	BEGIN
		--StatusIds for Inactive:-1,  Experimental:-5
		RAISERROR('To Delete a Project, Project must be Inactive/Experimental',16,1)
	END
	ELSE IF EXISTS (SELECT TOP 1 1 FROM dbo.ChargeCode cc INNER JOIN dbo.TimeEntry TE on TE.ChargeCodeId = cc.Id AND cc.ProjectId = @ProjectID)
	BEGIN
		RAISERROR ('This project cannot be deleted, because there are time entries related to it.', 16, 1)
	END
	ELSE IF EXISTS (SELECT 1 FROM dbo.DefaultMilestoneSetting WHERE ProjectId = @ProjectID)
	BEGIN
		RAISERROR ('This project cannot be deleted, because it is set as Default Milestone Project',16,1)
	END
	ELSE IF EXISTS (SELECT 1 FROM dbo.ProjectExpense WHERE ProjectId = @ProjectID)
	BEGIN
		RAISERROR ('This project cannot be deleted, because there are Expenses related to it.', 16, 1)
	END
	ELSE 
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION projectDelete
			
			-- Start logging session
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
			
			DELETE dbo.PersonTimeEntryRecursiveSelection  WHERE ProjectId = @ProjectID

			--Delete chargeCode related to the project
			IF EXISTS (SELECT TOP 1 1 FROM dbo.ChargeCode cc WHERE cc.ProjectId = @ProjectID)
			BEGIN

				DELETE cch
				FROM dbo.ChargeCodeTurnOffHistory cch INNER JOIN dbo.ChargeCode cc ON cch.ChargeCodeId = cc.Id WHERE cc.ProjectId = @ProjectID

				DELETE cc 
				FROM dbo.ChargeCode cc WHERE cc.ProjectId = @ProjectID

			END

			--Delete Project time Types if exists.
			DELETE PTT
			FROM ProjectTimeType PTT
			WHERE PTT.ProjectId = @ProjectID

			-- Delete Milestones of this project.
			IF EXISTS (SELECT 1 FROM Milestone WHERE ProjectId = @ProjectID)
			BEGIN
				DECLARE @milestones TABLE (RowId INT IDENTITY(1,1), milestoneID INT)
				DECLARE @index INT
				DECLARE @milestoneCount INT
				
				INSERT INTO @milestones(milestoneID)
				(SELECT MilestoneId FROM Milestone WHERE ProjectId = @ProjectID)
				
				SET @index = 1
				SET @milestoneCount = (SELECT COUNT(*) FROM @milestones)
				
				WHILE @index <= @milestoneCount
				BEGIN
					DECLARE @milestone INT
					SET @milestone = (SELECT milestoneId FROM @milestones WHERE RowId = @index)
					EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
					EXEC MilestoneDelete @milestone, @UserLogin 
					
					SET @index = @index + 1
				END
			END
		
			IF EXISTS (SELECT 1 FROM Opportunity WHERE ProjectId = @ProjectID)
			BEGIN
				EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
				UPDATE Opportunity
				SET ProjectId = NULL,
					LastUpdated = GETDATE()
				WHERE ProjectId = @ProjectID
			END
			
			IF EXISTS (SELECT ProjectId FROM Commission WHERE ProjectId = @ProjectID)
			BEGIN
				DELETE Commission
				WHERE ProjectId = @ProjectID
			END
			
			IF EXISTS (SELECT ProjectId FROM ProjectAttachment WHERE ProjectId = @ProjectID)
			BEGIN
				EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
				DELETE 
				FROM	ProjectAttachment
				WHERE ProjectId = @ProjectID
			END

			IF EXISTS (SELECT 1 FROM Note WHERE NoteTargetId = 2 AND TargetId = @ProjectID)
			BEGIN
				EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
				DELETE 
				FROM Note
				WHERE NoteTargetId = 2 AND TargetId = @ProjectID-- Here 2 is Project in NoteTarget table.
				EXEC dbo.SessionLogUnprepare
			END
			
			IF EXISTS (SELECT ProjectId FROM ProjectManagers WHERE ProjectId = @ProjectID)
			BEGIN
				DELETE 
				FROM	ProjectManagers
				WHERE ProjectId = @ProjectID
			END

			IF EXISTS (SELECT ProjectId FROM Project WHERE ProjectId = @ProjectID)
			BEGIN
				EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
				DELETE Project
				WHERE ProjectId = @ProjectID
			END
			
			-- End logging session
			EXEC dbo.SessionLogUnprepare
			
			COMMIT TRAN projectDelete
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN projectDelete
			SELECT @ErrorMessage = ERROR_MESSAGE()

			IF @ErrorMessage = [dbo].[GetErrorMessage](70018)
			BEGIN
				SET @ErrorMessage = 'This project cannot be deleted, because there are project expenses related to it.'
			END
			
			RAISERROR(@ErrorMessage, 16, 1) --To Raise IF any errors in MilestoneDelete,
		END CATCH
	END
END

