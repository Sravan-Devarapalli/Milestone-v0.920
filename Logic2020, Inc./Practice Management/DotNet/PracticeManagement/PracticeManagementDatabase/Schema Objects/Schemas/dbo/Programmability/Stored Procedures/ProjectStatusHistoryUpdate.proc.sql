CREATE PROCEDURE [dbo].[ProjectStatusHistoryUpdate]
(
	@ProjectId			INT,
	@ProjectStatusId	INT
)
AS
BEGIN
 DECLARE @Today DATETIME
	 SET @Today  = CONVERT(DATE,[dbo].[GettingPMTime](GETDATE()))
	 
	 -- Set the end date of the previous project status record to yester day
	 IF NOT EXISTS (SELECT 1 FROM dbo.ProjectStatusHistory 
					WHERE EndDate IS NULL 
						AND  ProjectId = @ProjectId
						AND  ProjectStatusId = @ProjectStatusId)
	 BEGIN
		 UPDATE dbo.ProjectStatusHistory
		 SET EndDate = @Today-1
		 WHERE EndDate IS NULL 
				AND  ProjectId = @ProjectId
				AND StartDate != @Today

		IF EXISTS (SELECT 1 FROM dbo.ProjectStatusHistory
					WHERE EndDate IS NULL 
								AND ProjectId = @ProjectId
								AND StartDate = @Today)
		BEGIN
			IF EXISTS (SELECT 1 FROM dbo.ProjectStatusHistory
					   WHERE EndDate = @Today-1
								AND ProjectId = @ProjectId
								AND ProjectStatusId = @ProjectStatusId)
			BEGIN
				UPDATE dbo.ProjectStatusHistory
				SET EndDate = NULL
				WHERE EndDate = @Today-1
					  AND ProjectId = @ProjectId
					  AND ProjectStatusId = @ProjectStatusId

				DELETE FROM  dbo.ProjectStatusHistory
				WHERE EndDate IS NULL 
					AND  ProjectId = @ProjectId
					AND  StartDate = @Today
			END
			ELSE
			BEGIN
				UPDATE dbo.ProjectStatusHistory
				SET ProjectStatusId = @ProjectStatusId
				WHERE EndDate IS NULL 
						AND  ProjectId = @ProjectId
						AND StartDate = @Today
			END
		END
		ELSE
		BEGIN	
			INSERT INTO [dbo].[ProjectStatusHistory]
			   ([ProjectId]
			   ,[ProjectStatusId]
			   ,[StartDate]
			   )
			VALUES
			   (@ProjectId
			   ,@ProjectStatusId
			   ,@Today
			   )
		END
	END
END
