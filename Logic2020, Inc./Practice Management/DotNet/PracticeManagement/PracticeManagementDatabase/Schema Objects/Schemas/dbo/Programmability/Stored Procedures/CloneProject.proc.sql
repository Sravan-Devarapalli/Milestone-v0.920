﻿CREATE PROCEDURE  [dbo].[CloneProject]
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
				 DirectorId, 
				 OpportunityId,
				 Description,
				 CanCreateCustomWorkTypes,
				 IsInternal,
				 IsNoteRequired,
				 ProjectOwnerId,
				 SalesPersonId,
				 PONumber,
				 SeniorManagerId)
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
						p.DirectorId,
						p.OpportunityId,
						p.Description,
						p.CanCreateCustomWorkTypes,
						p.IsInternal,
						p.IsNoteRequired,
						p.ProjectOwnerId,
						p.SalesPersonId,
						p.PONumber,
						p.SeniorManagerId
                FROM    dbo.Project AS p
                WHERE   p.ProjectId = @projectId
                
        SET @ClonedProjectId = SCOPE_IDENTITY()

		INSERT INTO dbo.ProjectTimeType
		SELECT @ClonedProjectId,TimeTypeId,IsAllowedToShow FROM dbo.ProjectTimeType WHERE ProjectId = @ProjectId

		INSERT INTO ProjectManagers(ProjectId,ProjectManagerId)
		SELECT  @ClonedProjectId,pm.ProjectManagerId
		FROM    dbo.ProjectManagers AS pm
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

        COMMIT TRANSACTION TR_CloneProject

    END

