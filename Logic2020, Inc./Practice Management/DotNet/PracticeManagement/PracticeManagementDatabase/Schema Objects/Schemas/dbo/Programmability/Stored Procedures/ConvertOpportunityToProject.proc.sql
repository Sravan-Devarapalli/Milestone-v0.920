CREATE PROCEDURE [dbo].[ConvertOpportunityToProject]
(
	@OpportunityId   INT,
	@UserLogin       NVARCHAR(255),
	@HasPersons		 BIT,
	@ProjectID		 INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ClientId INT
	DECLARE @Discount DECIMAL(18,2)
	DECLARE @Terms INT
	DECLARE @PracticeId INT
	DECLARE @Name NVARCHAR(100)
	DECLARE @ProjectedStartDate DATETIME
	DECLARE @ProjectedEndDate DATETIME
	DECLARE @MilestoneId INT
	DECLARE @GroupId INT
	DECLARE @SalespersonId INT
	DECLARE @OpportunityStatusId INT
	DECLARE @BuyerName NVARCHAR(100)
	DECLARE @IsChargeable BIT
	DECLARE @ProjectManagerId INT
	DECLARE @OpportunityPerson NVARCHAR(100)
	DECLARE @MilestonePersonId INT
	
	SELECT TOP 1
		   @ClientId = o.ClientId,
	       @Discount = o.Discount,
	       @Terms = o.Terms,
	       @PracticeId = o.PracticeId,
	       @Name = o.Name,
	       @ProjectedStartDate = o.ProjectedStartDate,
	       @ProjectedEndDate = o.ProjectedEndDate,
	       @SalespersonId = o.SalespersonId,
	       @OpportunityStatusId = o.OpportunityStatusId,
	       @BuyerName = o.BuyerName,
	       @GroupId = pg.GroupId,
	       @IsChargeable = 1,
	       @ProjectManagerId = ISNULL(o.OwnerId, pr.PracticeManagerId)
	  FROM dbo.v_Opportunity AS o
	  inner join dbo.ProjectGroup as pg on pg.ClientId = o.ClientId
	  inner join dbo.Practice as pr on pr.PracticeId = o.PracticeId
	 WHERE o.OpportunityId = @OpportunityId

	IF @OpportunityStatusId = 4 /* Won */
	BEGIN
		RAISERROR('Cannot convert an opportunity with the status Won to project.', 16, 1)
		RETURN
	END

	-- Create a project
	EXEC dbo.ProjectInsert @ClientId = @ClientId,
		@Discount = @Discount,
		@Terms = @Terms,
		@Name = @Name,
		@PracticeId = @PracticeId,
		@ProjectStatusId = 2 /* Projected */,
		@BuyerName = @BuyerName,
		@UserLogin = @UserLogin,
		@GroupId = @GroupId,
		@IsChargeable = @IsChargeable,
		@ProjectManagerId = @ProjectManagerId,
	    @ProjectId = @ProjectId OUTPUT,
		@OpportunityId = @OpportunityId

	IF(@HasPersons = 1)
	BEGIN
	
	-- Create a milestone
	EXEC dbo.MilestoneInsert @ProjectId = @ProjectId,
		@Description = 'Milestone 1',
		@Amount = NULL,
		@StartDate = @ProjectedStartDate,
		@ProjectedDeliveryDate = @ProjectedEndDate,
		@ActualDeliveryDate = NULL,
		@IsHourlyAmount = 1,
		@UserLogin = @UserLogin,
		@ConsultantsCanAdjust = 0,
		@IsChargeable = @IsChargeable,
		@MilestoneId = @MilestoneId OUTPUT
	
	-- Add persons to milestone
		DECLARE @OpportunityPersons TABLE(OpportunityId INT, PersonId INT, RowNumber INT)
		DECLARE @PersonsCount INT = (SELECT COUNT(PersonId) FROM dbo.OpportunityPersons WHERE OpportunityId = @OpportunityId AND OpportunityPersonTypeId = 1)
		DECLARE @tempPersonId INT
		DECLARE @Index INT = 1
		
		INSERT INTO @OpportunityPersons(RowNumber, OpportunityId, PersonId)
		SELECT	ROW_NUMBER() OVER(ORDER BY personId) AS RowNumber
				, OpportunityId
				, PersonId
		FROM dbo.OpportunityPersons
		WHERE OpportunityId = @OpportunityId AND OpportunityPersonTypeId = 1
				
		WHILE @Index <= @PersonsCount
		BEGIN
			SELECT @tempPersonId = PersonId FROM @OpportunityPersons WHERE RowNumber = @Index
			
			EXEC dbo.MilestonePersonInsert  @MilestoneId = @MilestoneId,
				@PersonId = @tempPersonId,
				@MilestonePersonId = @MilestonePersonId OUTPUT
			
			EXEC dbo.MilestonePersonEntryInsert @PersonId = @tempPersonId,
				@MilestonePersonId = @MilestonePersonId, 
				@StartDate= @ProjectedStartDate, 
				@EndDate = @ProjectedEndDate, 
				@HoursPerDay = 8,
				@PersonRoleId = NULL,
				@Amount = NULL,
				@Location = NULL,
				@UserLogin = @UserLogin
			
			SET @Index = @Index + 1;
		END 
	END

	-- Set opportunity status
	UPDATE dbo.Opportunity
	   SET OpportunityStatusId = 4 /* Won */
	       ,ProjectId = @ProjectID
	 WHERE OpportunityId = @OpportunityId

	DECLARE @PersonId INT
	SELECT @PersonId = p.PersonId FROM dbo.Person AS p WHERE p.Alias = @UserLogin

	DECLARE @CreatedMessage NVARCHAR(100)
	SELECT @CreatedMessage = 'Opportunity Won - Project created: ' + ProjectNumber
	  FROM dbo.Project AS p
	 WHERE p.ProjectId = @ProjectId

	-- Add an opportunity history
	EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
		@OpportunityTransitionStatusId = 2,
		@PersonId = @PersonId,
		@NoteText = @CreatedMessage,
		@OpportunityTransitionId = null
END
