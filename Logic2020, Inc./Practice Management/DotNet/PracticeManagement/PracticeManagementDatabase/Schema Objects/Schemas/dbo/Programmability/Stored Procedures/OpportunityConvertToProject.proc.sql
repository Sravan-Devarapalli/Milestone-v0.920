CREATE PROCEDURE dbo.OpportunityConvertToProject
(
	@OpportunityId   INT,
	@UserLogin       NVARCHAR(255),
	@ProjectID		 INT OUTPUT
)
AS
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
	
	SELECT TOP 1
		   @ClientId = o.ClientId,
	       @Discount = o.Discount,
	       @Terms = o.Terms,
	       @PracticeId = o.PracticeId,
	       @Name = o.Name,
	       @ProjectedStartDate = ISNULL(o.ProjectedStartDate, '1753-01-01'),
	       @ProjectedEndDate = ISNULL(o.ProjectedEndDate, '1753-01-01'),
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

	-- Add a sales commission
	INSERT INTO dbo.Commission
	            (ProjectId, PersonId, FractionOfMargin, CommissionType, MarginTypeId)
	     SELECT TOP 1 @ProjectId, c.PersonId, c.FractionOfMargin, 1 /* sales commission*/, 2 /* Sub-ordinate person margin */
	       FROM dbo.DefaultCommission AS c
	      WHERE c.PersonId = @SalespersonId
	        AND GETDATE() BETWEEN c.StartDate AND c.EndDate

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

	-- Set opportunity status
	UPDATE dbo.Opportunity
	   SET OpportunityStatusId = 4 /* Won */
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

