CREATE PROCEDURE  [dbo].[CloneProject]
    @ProjectId INT,
	@ProjectStatusId INT,
    @CloneMilestones BIT = 1,
    @CloneCommissions BIT = 1,
    @ClonedProjectId INT OUT
AS 
    BEGIN
        SET NOCOUNT ON ;

        BEGIN TRANSACTION TR_CloneProject

		-- Generating Project Number
        DECLARE @ProjectNumber NVARCHAR(12),@IsInternal BIT 
	    SELECT @IsInternal = IsInternal FROM dbo.Project WHERE ProjectId = @ProjectId
		EXEC dbo.GenerateNewProjectNumber @IsInternal, @ProjectNumber OUTPUT ;

		DECLARE @Today DATETIME
		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

        INSERT INTO dbo.Project
	            (ClientId, 
				 Discount, 
				 Terms, 
				 Name, 
				 PracticeId,
				 StartDate,
				 EndDate,
	             ProjectStatusId,
				 ProjectNumber, 
				 BuyerName, 
				 GroupId,
				 PricingListId, 
				 BusinessTypeId,
				 IsChargeable, 
				 ExecutiveInChargeId, 
				 OpportunityId,
				 Description,
				 CanCreateCustomWorkTypes,
				 IsInternal,
				 IsNoteRequired,
				 ProjectManagerId,
				 SalesPersonId,
				 PONumber,
				 EngagementManagerId,
				 DivisionId,
				 ChannelId,
				 SubChannel,
				 RevenueTypeId,
				 OfferingId,
				 PreviousProjectNumber,
				 OutsourceId,
				 IsClientTimeEntryRequired
				 )
                SELECT  p.ClientId,
                        p.Discount,
                        p.Terms,
                        SUBSTRING(p.[Name] + ' (cloned)', 0, 100),
                        p.PracticeId,
                        NULL, --StartDate ,
                        NULL, --EndDate ,
                        @ProjectStatusId,
                        @ProjectNumber,
                        p.BuyerName,
                        p.GroupId,
						p.PricingListId,
						p.BusinessTypeId,
                        p.IsChargeable,                     
						p.ExecutiveInChargeId,
						p.OpportunityId,
						p.Description,
						p.CanCreateCustomWorkTypes,
						p.IsInternal,
						p.IsNoteRequired,
						p.ProjectManagerId,
						p.SalesPersonId,
						p.PONumber,
						p.EngagementManagerId,
						p.DivisionId,
						p.ChannelId,
						p.SubChannel,
						p.RevenueTypeId,
						p.OfferingId,
						p.PreviousProjectNumber,
						p.OutsourceId,
						p.IsClientTimeEntryRequired
                FROM    dbo.Project AS p
                WHERE   p.ProjectId = @projectId
                
        SET @ClonedProjectId = SCOPE_IDENTITY()

		INSERT INTO dbo.ProjectTimeType
		SELECT @ClonedProjectId,TimeTypeId,IsAllowedToShow FROM dbo.ProjectTimeType WHERE ProjectId = @ProjectId

		INSERT INTO ProjectAccess(ProjectId,ProjectAccessId)
		SELECT  @ClonedProjectId,pm.ProjectAccessId
		FROM    dbo.ProjectAccess AS pm
        WHERE   pm.ProjectId = @projectId

		INSERT INTO dbo.ProjectCapabilities(ProjectId,CapabilityId)
		SELECT  @ClonedProjectId,PC.CapabilityId
		FROM    dbo.ProjectCapabilities PC
	    WHERE   PC.ProjectId = @projectId
              
        IF @CloneMilestones = 1 
            BEGIN

                DECLARE projectMilestone CURSOR
                    FOR SELECT  MilestoneId
                        FROM    dbo.Milestone
                        WHERE   ProjectId = @projectId

                DECLARE @origMilestoneId INT 

                OPEN projectMilestone  
                FETCH NEXT FROM projectMilestone INTO @origMilestoneId  

                WHILE @@FETCH_STATUS = 0 
                    BEGIN  
    
                        EXECUTE dbo.MilestoneClone @MilestoneId = @origMilestoneId,
                            @CloneDuration = 0, @MilestoneCloneId = 0,
                            @ProjectId = @ClonedProjectId

                        FETCH NEXT FROM projectMilestone INTO @origMilestoneId  
                    END  

                CLOSE projectMilestone  
                DEALLOCATE projectMilestone
            END 
            
        IF @CloneCommissions = 1 AND @CloneMilestones = 1 
            BEGIN
			
			INSERT INTO dbo.Attribution(AttributionRecordTypeId,AttributionTypeId,ProjectId,TargetId,StartDate,EndDate,Percentage)
			SELECT	A.AttributionRecordTypeId,
					A.AttributionTypeId,
					@ClonedProjectId,
					A.TargetId,
					A.StartDate,
					A.EndDate,
					A.Percentage
			FROM	dbo.Attribution A
			WHERE	A.ProjectId = @projectId
         	
            END

		UPDATE dbo.Project
		SET CreatedDate = @Today
		WHERE ProjectId = @ClonedProjectId

        COMMIT TRANSACTION TR_CloneProject

    END

