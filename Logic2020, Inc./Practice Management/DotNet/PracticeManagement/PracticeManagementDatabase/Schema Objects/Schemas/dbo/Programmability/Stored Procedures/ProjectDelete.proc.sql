CREATE PROCEDURE [dbo].[ProjectDelete]
(
	@ProjectID INT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN
	DECLARE @ErrorMessage NVARCHAR(MAX)
		
	IF NOT EXISTS (SELECT 1 FROM Project WHERE ProjectId = @ProjectID AND ProjectStatusId IN (1, 5))
	BEGIN
		--StatusIds for Inactive:-1,  Experimental:-5
		RAISERROR('To Delete a Project, Project must be Inactive/Experimental',16,1)
	END
	ELSE IF EXISTS (SELECT 1 FROM v_TimeEntries WHERE ProjectId = @ProjectID)
	BEGIN
		RAISERROR ('Project persons entered Time entries towards this project.', 16, 1)
	END
	ELSE IF EXISTS (SELECT 1 FROM DefaultMilestoneSetting WHERE ProjectId = @ProjectID)
	BEGIN
		RAISERROR ('This project set as Default Milestone Project',16,1)
	END
	ELSE 
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION projectDelete
			
			-- Start logging session
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
			
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
					EXEC MilestoneDelete @milestone, @UserLogin 
					
					SET @index = @index + 1
				END
			END
		
			IF EXISTS (SELECT 1 FROM Opportunity WHERE ProjectId = @ProjectID)
			BEGIN
				UPDATE Opportunity
				SET ProjectId = NULL
				WHERE ProjectId = @ProjectID
			END
			
			IF EXISTS (SELECT ProjectId FROM Commission WHERE ProjectId = @ProjectID)
			BEGIN
				DELETE Commission
				WHERE ProjectId = @ProjectID
			END
			
			IF EXISTS (SELECT ProjectId FROM ProjectAttachment WHERE ProjectId = @ProjectID)
			BEGIN
				DELETE 
				FROM	ProjectAttachment
				WHERE ProjectId = @ProjectID
			END
			
			IF EXISTS (SELECT ProjectId FROM ProjectBillingInfo WHERE ProjectId = @ProjectID)
			BEGIN
				IF EXISTS (SELECT 1 FROM Note WHERE NoteTargetId = 5 AND TargetId = @ProjectID)
				BEGIN--To delete ProjectBillingInfoNote.
					DELETE Note
					WHERE NoteTargetId = 5 AND TargetId = @ProjectID-- Here 5 is ProjectBillingInfo in NoteTarget table.
				END
				
				DELETE
				FROM ProjectBillingInfo
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
			
			IF EXISTS (SELECT ProjectId FROM Project WHERE ProjectId = @ProjectID)
			BEGIN
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
			
			RAISERROR(@ErrorMessage, 16, 1) --To Raise IF any errors in MilestoneDelete,
		END CATCH
	END
END
