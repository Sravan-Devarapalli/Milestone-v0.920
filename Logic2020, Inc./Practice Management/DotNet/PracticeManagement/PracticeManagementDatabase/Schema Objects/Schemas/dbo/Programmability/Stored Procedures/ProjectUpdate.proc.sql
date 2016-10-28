---------------------------
-- Updated By:	Srinivas.M
-- Updated Date: 2012-06-05
---------------------------
CREATE PROCEDURE dbo.ProjectUpdate
(
	@ProjectId          INT,
	@ClientId           INT,
	@Terms              INT,
	@Name               NVARCHAR(100),
	@PracticeId         INT,
	@ProjectStatusId    INT,
	@BuyerName          NVARCHAR(100),
	@UserLogin          NVARCHAR(255),
	@GroupId			INT,
	@DirectorId			INT,
	@ProjectManagerIdsList	NVARCHAR(MAX),
	@Description           NVARCHAR(MAX),
	@CanCreateCustomWorkTypes BIT,
	@IsInternal			BIT,
	@IsNoteRequired     BIT = 1  ,
	@IsClientTimeEntryRequired BIT=1,
	@ProjectOwner       INT,
	@SowBudget			DECIMAL(18,2),
	@POAmount			DECIMAL(18,2),
	@ProjectCapabilityIds NVARCHAR(MAX),
	@PricingListId      INT = NULL,
	@BusinessTypeId     INT= NULL,
	@SeniorManagerId			INT = NULL,
	@IsSeniorManagerUnassigned  BIT = 0,
	@CSATOwnerId			INT = NULL,
	@PONumber			NVARCHAR(100) = NULL,
	@SalesPersonId			INT = NULL,
	@DivisionId         INT,
	@ChannelId          INT,
	@SubChannel			NVARCHAR(100),
	@RevenueTypeId		INT,
	@OfferingId			INT,
	@PreviousProjectNumber NVARCHAR (12) =NULL,
	@OutsourceId        INT
)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRY
	BEGIN TRAN  ProjectUpdate

		DECLARE @Persons TABLE(Id INT, 
							   IsDoneExecuting BIT)
		DECLARE @UpdatedBy INT 
		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		SELECT @UpdatedBy = PersonId FROM dbo.Person WHERE Alias = @UserLogin
		IF EXISTS (SELECT 1 FROM dbo.Project WHERE ProjectId = @ProjectId AND IsInternal != @IsInternal)
		BEGIN
			IF EXISTS (SELECT 1	FROM dbo.TimeType tt 
						INNER JOIN dbo.ChargeCode cc ON tt.TimeTypeId = cc.TimeTypeId 
													AND cc.ProjectID = @ProjectId 
													AND tt.IsInternal != @IsInternal 
													AND tt.IsDefault = 0 
						INNER JOIN dbo.TimeEntry te ON cc.ID = te.ChargeCodeId )
			BEGIN
				RAISERROR ('Can not change project status as some work types are already in use.', 16, 1)
			END
			--to delete existing project time types
			DELETE ptt 
			FROM dbo.ProjectTimeType ptt 
			WHERE ptt.ProjectId = @ProjectId 
		END	

		DECLARE @PreviousClientId INT, @PreviousGroupId INT,@PreviousProjectStatusId INT,@CurrentPMTime DATETIME

		SELECT @PreviousClientId = ClientId, @PreviousGroupId = GroupId,@PreviousProjectStatusId=ProjectStatusId,@CurrentPMTime = CONVERT(DATE,dbo.[GettingPMTime](GETUTCDATE()))
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
		IF (@ProjectStatusId != 3 AND @ProjectStatusId != 6 AND @ProjectStatusId != 8) -- add AT risk
		BEGIN
			 DELETE [dbo].[PersonTimeEntryRecursiveSelection]
   			 WHERE [ProjectId] = @ProjectId
		END

		EXEC dbo.SessionLogUnprepare
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		UPDATE P
			SET ClientId		= @ClientId,
				Terms			= @Terms,
				NAME			= @Name,
				PracticeId		= @PracticeId,
				ProjectStatusId	= @ProjectStatusId,
				BuyerName		= @BuyerName,
				GroupId			= @GroupId,
				ExecutiveInChargeId		= @DirectorId,
				Description		=@Description,
				CanCreateCustomWorkTypes = @CanCreateCustomWorkTypes,
				IsInternal		=@IsInternal,
				IsNoteRequired  = @IsNoteRequired,
				IsClientTimeEntryRequired=@IsClientTimeEntryRequired,
				ProjectManagerId  = @ProjectOwner,
				SowBudget		= @SowBudget,
				POAmount        = @POAmount,
				PricingListId   = @PricingListId,
				BusinessTypeId  = @BusinessTypeId,
				EngagementManagerId = @SeniorManagerId,
				IsSeniorManagerUnassigned  = @IsSeniorManagerUnassigned,
				ReviewerId = @CSATOwnerId,
				PONumber		= @PONumber,
				SalesPersonId = @SalesPersonId,
				DivisionId=@DivisionId,
				ChannelId=@ChannelId,
				SubChannel=@SubChannel,
				RevenueTypeId=@RevenueTypeId,
				OfferingId=@OfferingId,
				PreviousProjectNumber=@PreviousProjectNumber,
				OutsourceId=@OutsourceId
		FROM dbo.Project P
		WHERE ProjectId = @ProjectId

		--If project status 
		IF((@ProjectStatusId IN (1,2,3,4,8) AND @PreviousProjectStatusId IN (5,6,7)) OR (@PreviousProjectStatusId IN (1,2,3,4,8) AND @ProjectStatusId IN (5,6,7)))
		BEGIN
			 INSERT INTO @Persons(Id,IsDoneExecuting)
			 SELECT DISTINCT mp.PersonId,0
			 FROM dbo.Milestone m
			 INNER JOIN dbo.MilestonePerson mp on mp.MilestoneId = m.MilestoneId
			 INNER JOIN dbo.MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
			 WHERE m.ProjectId = @ProjectId AND mpe.IsBadgeRequired = 1 AND mpe.IsApproved = 1

			 WHILE EXISTS(SELECT 1 FROM @Persons WHERE IsDoneExecuting=0)
			 BEGIN
			    DECLARE @PersonId INT
				SET @PersonId =(SELECT TOP 1 Id FROM @Persons WHERE IsDoneExecuting = 0)
				EXEC [dbo].[UpdateMSBadgeDetailsByPersonId]	@PersonId = @PersonId,@UpdatedBy =@UpdatedBy

				UPDATE @Persons
				SET IsDoneExecuting = 1
				WHERE Id = @PersonId

			 END
		END

		IF @ClientId <> @PreviousClientId OR @ProjectStatusId <> 4
		BEGIN
				UPDATE P
				SET P.Discount = C.DefaultDiscount
				FROM dbo.Project P
				JOIN dbo.Client C ON C.ClientId = P.ClientId
				WHERE ProjectId = @ProjectId
				AND P.Discount <> C.DefaultDiscount
		END

		EXEC dbo.ProjectStatusHistoryUpdate
			@ProjectId = @ProjectId,
			@ProjectStatusId = @ProjectStatusId

		--if Review enddate is greater than current time when the project is set to completed.
		UPDATE CSAT
		SET CSAT.ReviewEndDate = @CurrentPMTime,
			CSAT.ReviewStartDate = CASE WHEN CSAT.ReviewStartDate > @CurrentPMTime THEN @CurrentPMTime ELSE CSAT.ReviewStartDate END
		FROM dbo.ProjectCSAT CSAT
		WHERE CSAT.ProjectId = @ProjectId 
				AND @ProjectStatusId != @PreviousProjectStatusId 
				AND @ProjectStatusId = 4 --completed Status
				AND CSAT.ReviewEndDate > @CurrentPMTime

		DECLARE @OpportunityId INT = NULL
		
		SELECT @OpportunityId = OpportunityId 
								 FROM  dbo.Project 
								 WHERE ProjectId = @ProjectId
	  
			
		IF(@OpportunityId IS NOT NULL)
		BEGIN
	  
		  UPDATE dbo.Opportunity 
		  SET Description = @Description,
		  BusinessTypeId = @BusinessTypeId,
		  PricingListId = @PricingListId
		  WHERE OpportunityId = @OpportunityId 
	 
		END


	    DELETE pm
		FROM dbo.ProjectAccess pm
		LEFT JOIN [dbo].ConvertStringListIntoTable(@ProjectManagerIdsList) AS p 
		ON pm.ProjectId = @ProjectId AND pm.ProjectAccessId = p.ResultId 
		WHERE p.ResultId IS NULL and pm.ProjectId = @ProjectId

		INSERT INTO dbo.ProjectAccess(ProjectId,ProjectAccessId)
		SELECT @ProjectId ,p.ResultId
		FROM [dbo].ConvertStringListIntoTable(@ProjectManagerIdsList) AS p 
		LEFT JOIN dbo.ProjectAccess pm
		ON p.ResultId = pm.ProjectAccessId AND pm.ProjectId=@ProjectId
		WHERE pm.ProjectAccessId IS NULL

		EXEC dbo.SessionLogUnprepare
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	    
		DELETE PC
		FROM dbo.ProjectCapabilities PC
		LEFT JOIN [dbo].ConvertStringListIntoTable(@ProjectCapabilityIds) AS p 
		ON PC.ProjectId = @ProjectId AND PC.CapabilityId = p.ResultId 
		WHERE p.ResultId IS NULL and PC.ProjectId = @ProjectId

		INSERT INTO dbo.ProjectCapabilities(ProjectId,CapabilityId)
		SELECT @ProjectId ,p.ResultId
		FROM [dbo].ConvertStringListIntoTable(@ProjectCapabilityIds) AS p 
		LEFT JOIN dbo.ProjectCapabilities PC
		ON p.ResultId = PC.CapabilityId AND PC.ProjectId=@ProjectId
		WHERE PC.CapabilityId IS NULL

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

