﻿
CREATE PROCEDURE dbo.ProjectUpdate
(
	@ProjectId          INT,
	@ClientId           INT,
	@Discount           DECIMAL(18,2),
	@Terms              INT,
	@Name               NVARCHAR(100),
	@PracticeId         INT,
	@ProjectStatusId    INT,
	@BuyerName          NVARCHAR(100),
	@UserLogin          NVARCHAR(255),
	@GroupId			INT,
	@IsChargeable		BIT,
	@DirectorId			INT,
	@ProjectManagerIdsList	NVARCHAR(MAX),
	@Description           NVARCHAR(MAX),
	@CanCreateCustomWorkTypes BIT,
	@IsInternal			BIT  
)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRAN  T1;

		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		IF EXISTS (SELECT 1 FROM dbo.Project WHERE ProjectId = @ProjectId AND IsInternal != @IsInternal)
		BEGIN
			IF EXISTS (SELECT 1	FROM dbo.TimeType tt 
						INNER JOIN dbo.CHARGECODE cc ON tt.TimeTypeId = cc.TimeTypeId 
													AND cc.ProjectID = @ProjectId 
													AND tt.IsInternal != @IsInternal 
													AND tt.IsDefault = 0 
						INNER JOIN dbo.TimeEntry te ON cc.ID = te.ChargeCodeId )
			BEGIN
				RAISERROR ('Can not change project status as some timetypes are already in use.', 16, 1)
			END
		--to delete existing projecttimetypes
		DELETE ptt 
		FROM dbo.ProjectTimeType ptt 
		WHERE ptt.ProjectId = @ProjectId 

		END

	

		UPDATE dbo.Project
			SET ClientId			= @ClientId,
				Discount			= @Discount,
				Terms			= @Terms,
				NAME				= @Name,
				PracticeId		= @PracticeId,
				ProjectStatusId	= @ProjectStatusId,
				BuyerName		= @BuyerName,
				GroupId			= @GroupId,
				IsChargeable		= @IsChargeable,
				DirectorId		= @DirectorId,
				Description		=@Description,
				CanCreateCustomWorkTypes = @CanCreateCustomWorkTypes,
				IsInternal		=@IsInternal
			WHERE ProjectId = @ProjectId

		DECLARE @OpportunityId INT = NULL
		
		SELECT @OpportunityId = OpportunityId 
								 FROM  dbo.Project 
								 WHERE ProjectId = @ProjectId
	  
			
		IF(@OpportunityId IS NOT NULL)
		BEGIN
	  
		  UPDATE dbo.Opportunity 
		  SET Description = @Description
		  WHERE OpportunityId = @OpportunityId 
	 
		END


	    DELETE pm
		FROM ProjectManagers pm
		LEFT JOIN [dbo].ConvertStringListIntoTable(@ProjectManagerIdsList) AS p 
		ON pm.ProjectId = @ProjectId AND pm.ProjectManagerId = p.ResultId 
		WHERE p.ResultId IS NULL and pm.ProjectId = @ProjectId

		INSERT INTO ProjectManagers(ProjectId,ProjectManagerId)
		SELECT @ProjectId ,p.ResultId
		FROM [dbo].ConvertStringListIntoTable(@ProjectManagerIdsList) AS p 
		LEFT JOIN ProjectManagers pm
		ON p.ResultId = pm.ProjectManagerId AND pm.ProjectId=@ProjectId
		WHERE pm.ProjectManagerId IS NULL



		-- End logging session
		EXEC dbo.SessionLogUnprepare

	COMMIT TRAN T1;	

END

