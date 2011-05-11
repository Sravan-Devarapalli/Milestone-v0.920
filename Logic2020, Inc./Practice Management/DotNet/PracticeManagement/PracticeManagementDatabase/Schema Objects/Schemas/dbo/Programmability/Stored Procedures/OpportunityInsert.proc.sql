CREATE PROCEDURE [dbo].[OpportunityInsert]
(
	@Name                  NVARCHAR(50),
	@ClientId              INT,
	@SalespersonId         INT,
	@OpportunityStatusId   INT,
	@PriorityId            INT,
	@ProjectedStartDate    DATETIME,
	@ProjectedEndDate      DATETIME,
	@Description           NVARCHAR(MAX),
	@PracticeId            INT,
	@BuyerName             NVARCHAR(100),
	@Pipeline              NVARCHAR(512),
	@Proposed              NVARCHAR(512),
	@SendOut               NVARCHAR(512),
	@UserLogin             NVARCHAR(255),
	@OpportunityId         INT OUTPUT,
	@OpportunityIndex      INT,
	@ProjectId             INT,
	@OwnerId	       INT = NULL,
	@GroupId	       INT,
	@EstimatedRevenue  DECIMAL(18,2) 
)
AS
BEGIN
	SET NOCOUNT ON

		-- Generating Opportunity Number
		DECLARE @OpportunityNumber NVARCHAR(12)
		DECLARE @StringCounter NVARCHAR(7)
		DECLARE @Counter INT

		SET @Counter = 0

		WHILE  (1 = 1)
		BEGIN

			SET @StringCounter = CAST(@Counter AS NVARCHAR(7))
			IF LEN ( @StringCounter ) = 1
				SET @StringCounter =  '0' + @StringCounter

			SET @OpportunityNumber = dbo.MakeNumberFromDate('O', GETDATE()) + @StringCounter
	
			IF (NOT EXISTS (SELECT 1 FROM [dbo].[Opportunity] as o WHERE o.[OpportunityNumber] = @OpportunityNumber) )
				BREAK

			SET @Counter = @Counter + 1
		END

		INSERT INTO dbo.Opportunity
					(Name, ClientId, SalespersonId, OpportunityStatusId, PriorityId,
					 ProjectedStartDate, ProjectedEndDate, OpportunityNumber, Description, PracticeId, BuyerName,
					 Pipeline, Proposed, SendOut, OpportunityIndex,  ProjectId, OwnerId, GroupId ,EstimatedRevenue)
			 VALUES (@Name, @ClientId, @SalespersonId, @OpportunityStatusId, @PriorityId,
					 @ProjectedStartDate, @ProjectedEndDate, @OpportunityNumber, @Description, @PracticeId, @BuyerName,
					 @Pipeline, @Proposed, @SendOut, @OpportunityIndex, @ProjectId, @OwnerId, @GroupId ,@EstimatedRevenue)

		SET @OpportunityId = SCOPE_IDENTITY()

		DECLARE @PersonId INT
		SELECT @PersonId = PersonId FROM dbo.Person WHERE Alias = @UserLogin

		DECLARE @CreatedMessage NVARCHAR(200)
		SELECT @CreatedMessage = 'Opportunity created ' + o.ClientName + ' ' + o.Name
		  FROM dbo.v_Opportunity AS o
		 WHERE o.OpportunityId = @OpportunityId

		EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
			@OpportunityTransitionStatusId = 1,
			@PersonId = @PersonId,
			@NoteText = @CreatedMessage,
			@OpportunityTransitionId = NULL
	
END
