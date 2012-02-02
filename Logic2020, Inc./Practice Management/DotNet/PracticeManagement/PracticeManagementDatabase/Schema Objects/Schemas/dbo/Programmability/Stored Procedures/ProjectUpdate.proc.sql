
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
	
	BEGIN TRY
	BEGIN TRAN  ProjectUpdate

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
				RAISERROR ('Can not change project status as some work types are already in use.', 16, 1)
			END
			--to delete existing projecttimetypes
			DELETE ptt 
			FROM dbo.ProjectTimeType ptt 
			WHERE ptt.ProjectId = @ProjectId 
		END	

		DECLARE @PreviousClientId INT, @PreviousGroupId INT

		SELECT @PreviousClientId = ClientId, @PreviousGroupId = GroupId
		FROM Project
		WHERE ProjectId = @ProjectId

		--If No timeEntries exists for the project then update exists chargeCode with new clientId or new ProjectGroupId.
		IF @ClientId <> @PreviousClientId OR @GroupId <> @PreviousGroupId
		BEGIN
			IF EXISTS (SELECT 1
						FROM dbo.ChargeCode CC
						INNER JOIN TimeEntry TE ON TE.ChargeCodeId = CC.Id AND CC.ProjectId = @ProjectId
						)
			BEGIN
				RAISERROR ('Can not change project account or business unit as some time entered towards this Account-BusinessUnit-Project.', 16, 1)
			END
			ELSE
			BEGIN
				UPDATE CC
					SET CC.ClientId = @ClientId,
						CC.ProjectGroupId = @GroupId
				FROM dbo.ChargeCode CC
				WHERE CC.ProjectId = @ProjectId
					AND ( CC.ClientId <> @ClientId OR CC.ProjectGroupId <> @GroupId )

				UPDATE PTRS
					SET PTRS.ClientId = @ClientId,
						PTRS.ProjectGroupId = @GroupId
				FROM dbo.PersonTimeEntryRecursiveSelection PTRS
				WHERE PTRS.ProjectId = @ProjectId
					AND ( PTRS.ClientId <> @ClientId OR PTRS.ProjectGroupId <> @GroupId )
			END
		END

		--if that projectstatus is other that active or internal recursive entries need to be removed
		IF (@ProjectStatusId != 3 AND @ProjectStatusId != 6)
		BEGIN
			 DELETE [dbo].[PersonTimeEntryRecursiveSelection]
   			 WHERE [ProjectId] = @ProjectId
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

	COMMIT TRAN ProjectUpdate
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN ProjectUpdate
		
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

END

