﻿-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-03-27
-- Updated By: ThulasiRam.P
-- Updated Date: 2012-05-21
-- Description:	Clones project specified
-- =============================================
CREATE PROCEDURE [dbo].[CloneProject]
    @ProjectId INT,
	@ProjectStatusId INT,
    @CloneMilestones BIT = 1,
    @CloneBillingInfo BIT = 1,
    @CloneNotes BIT = 1,
    @CloneCommissions BIT = 1,
    @ClonedProjectId INT OUT
AS 
    BEGIN
        SET NOCOUNT ON ;

        BEGIN TRANSACTION

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
				 IsChargeable, 
				 DirectorId, 
				 OpportunityId,
				 Description,
				 CanCreateCustomWorkTypes,
				 IsInternal,
				 IsNoteRequired,
				 ProjectOwnerId)
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
                        p.IsChargeable,                     
						p.DirectorId,
						p.OpportunityId,
						p.Description,
						p.CanCreateCustomWorkTypes,
						p.IsInternal,
						p.IsNoteRequired,
						p.ProjectOwnerId
                FROM    dbo.Project AS p
                WHERE   p.ProjectId = @projectId
                
        SET @ClonedProjectId = SCOPE_IDENTITY()

		INSERT INTO dbo.ProjectTimeType
		SELECT @ClonedProjectId,TimeTypeId,IsAllowedToShow FROM dbo.ProjectTimeType WHERE ProjectId = @ProjectId

		INSERT INTO ProjectManagers(ProjectId,ProjectManagerId)
		SELECT  @ClonedProjectId,pm.ProjectManagerId
		FROM    dbo.ProjectManagers AS pm
        WHERE   pm.ProjectId = @projectId
          
        IF @CloneNotes = 1
        BEGIN
			exec dbo.NotesClone @OldTargetId = @ProjectId, @NewTargetId = @ClonedProjectId
        END
                
        IF @CloneBillingInfo = 1
            AND EXISTS ( SELECT 1
                         FROM   dbo.ProjectBillingInfo
                         WHERE  ProjectId = @ProjectId ) 
            BEGIN
                DECLARE @BillingContact NVARCHAR(100)
                DECLARE @BillingPhone NVARCHAR(25)
                DECLARE @BillingEmail NVARCHAR(100)
                DECLARE @BillingType NVARCHAR(25)
                DECLARE @BillingAddress1 NVARCHAR(100)
                DECLARE @BillingAddress2 NVARCHAR(100)
                DECLARE @BillingCity NVARCHAR(50)
                DECLARE @BillingState NVARCHAR(50)
                DECLARE @BillingZip NVARCHAR(10)
                DECLARE @PurchaseOrder NVARCHAR(25)
	
                SELECT  @BillingContact = BillingContact,
                        @BillingPhone = BillingPhone,
                        @BillingEmail = BillingEmail,
                        @BillingType = BillingType,
                        @BillingAddress1 = BillingAddress1,
                        @BillingAddress2 = BillingAddress2,
                        @BillingCity = BillingCity,
                        @BillingState = BillingState,
                        @BillingZip = BillingZip,
                        @PurchaseOrder = PurchaseOrder
                FROM    dbo.ProjectBillingInfo
                WHERE   ProjectId = @ProjectId
                
                EXEC dbo.ProjectBillingInfoSave @ProjectId = @ClonedProjectId, -- int
                    @BillingContact = @BillingContact, -- nvarchar(100)
                    @BillingPhone = @BillingPhone, -- nvarchar(25)
                    @BillingEmail = @BillingEmail, -- nvarchar(100)
                    @BillingType = @BillingType, -- nvarchar(25)
                    @BillingAddress1 = @BillingAddress1, -- nvarchar(100)
                    @BillingAddress2 = @BillingAddress2, -- nvarchar(100)
                    @BillingCity = @BillingCity, -- nvarchar(50)
                    @BillingState = @BillingState, -- nvarchar(50)
                    @BillingZip = @BillingZip, -- nvarchar(10)
                    @PurchaseOrder = @PurchaseOrder -- nvarchar(25)
                
            END            
               
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
            
        IF @CloneCommissions = 1
            AND EXISTS ( SELECT 1
                         FROM   dbo.Commission
                         WHERE  ProjectId = @ProjectId ) 
            BEGIN
         	
                INSERT  INTO dbo.Commission
                        (
                          ProjectId,
                          PersonId,
                          FractionOfMargin,
                          CommissionType,
                          ExpectedDatePaid,
                          ActualDatePaid,
                          MarginTypeId
         		        
                        )
                        SELECT  @ClonedProjectId,
                                PersonId,
                                FractionOfMargin,
                                CommissionType,
                                ExpectedDatePaid,
                                ActualDatePaid,
                                MarginTypeId
                        FROM    dbo.Commission
                        WHERE   ProjectId = @ProjectId
         	
            END

        COMMIT

    END

