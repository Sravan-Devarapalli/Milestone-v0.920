
CREATE PROCEDURE [dbo].[OpportunityUpdate]
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
	@OpportunityId         INT,
	@UserLogin             NVARCHAR(255),
    @ProjectId             INT,
    @OpportunityIndex      INT,
	@OwnerId			   INT = NULL,
	@GroupId			   INT,
	@EstimatedRevenue	   DECIMAL(18,2),
	@PersonIdList          NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON
		DECLARE @PrevOpportunityStatusId INT
		DECLARE @PrevPriority NVARCHAR(255)
		DECLARE @PrevPriorityId INT
		DECLARE @PrevPipeline NVARCHAR(1000)
		DECLARE @PrevProposed NVARCHAR(1000)
		DECLARE @PrevSendOut NVARCHAR(1000)
		DECLARE @CurrentPriority NVARCHAR(255)

		SELECT @PrevOpportunityStatusId = o.OpportunityStatusId,
			   @PrevPriority = OP.Priority,
			   @PrevPriorityId = O.PriorityId,
			   @PrevPipeline = o.Pipeline,
			   @PrevProposed = o.Proposed,
			   @PrevSendOut = o.SendOut
		  FROM dbo.Opportunity AS o
		  JOIN dbo.OpportunityPriorities AS OP ON Op.Id = o.PriorityId
		 WHERE o.OpportunityId = @OpportunityId

		 SELECT @CurrentPriority = Priority
		 FROM dbo.OpportunityPriorities
		 WHERE Id = @PriorityId

		UPDATE dbo.Opportunity
		   SET Name = @Name,
			   ClientId = @ClientId,
			   SalespersonId = @SalespersonId,
			   OpportunityStatusId = @OpportunityStatusId,
			   PriorityId = @PriorityId,
			   ProjectedStartDate = @ProjectedStartDate,
			   ProjectedEndDate = @ProjectedEndDate,
			   Description = @Description,
			   PracticeId = @PracticeId,
			   BuyerName = @BuyerName,
			   Pipeline = @Pipeline,
			   Proposed = @Proposed,
			   SendOut = @SendOut,
			   ProjectId = @ProjectId,
			   OpportunityIndex	= @OpportunityIndex,
			   OwnerId = @OwnerId,
			   GroupId = @GroupId,
			   EstimatedRevenue = @EstimatedRevenue
		 WHERE OpportunityId = @OpportunityId

		-- Logging changes
		DECLARE @NoteText NVARCHAR(2000)
		DECLARE @PersonId INT
		SELECT @PersonId = PersonId FROM dbo.Person WHERE Alias = @UserLogin

		-- Determine if the status was changed
		IF @PrevOpportunityStatusId <> @OpportunityStatusId
		BEGIN
			-- Create a history record
			SELECT @NoteText = 'Status changed.  Was: ' + Name
			  FROM dbo.OpportunityStatus
			 WHERE OpportunityStatusId = @PrevOpportunityStatusId

			SELECT @NoteText = @NoteText + ' now: ' + Name
			  FROM dbo.OpportunityStatus
			 WHERE OpportunityStatusId = @OpportunityStatusId

			EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
				@OpportunityTransitionStatusId = 2 /* Notes */,
				@PersonId = @PersonId,
				@NoteText = @NoteText
		END

		-- Determine if the priority was changed
		IF @PrevPriorityId <> @PriorityId
		BEGIN
			-- Create a history record
			SET @NoteText = 'Priority changed.  Was: ' + @PrevPriority + ' now: ' + @CurrentPriority

			EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
				@OpportunityTransitionStatusId = 2 /* Notes */,
				@PersonId = @PersonId,
				@NoteText = @NoteText
		END

		-- Determine if the pipeline was changed
		IF ISNULL(@PrevPipeline, '') <> ISNULL(@Pipeline, '')
		BEGIN
			-- Create a history record
			SET @NoteText = 'Pipeline was: ' + ISNULL(@PrevPipeline, '') + ' now: ' + ISNULL(@Pipeline, '')

			EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
				@OpportunityTransitionStatusId = 6 /* Pipeline */,
				@PersonId = @PersonId,
				@NoteText = @NoteText
		END

		-- Determine if the proposed was changed
		IF ISNULL(@PrevProposed, '') <> ISNULL(@Proposed, '')
		BEGIN
			-- Create a history record
			SET @NoteText = 'Proposed was: ' + ISNULL(@PrevProposed, '') + ' now: ' + ISNULL(@Proposed, '')

			EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
				@OpportunityTransitionStatusId = 3 /* Proposed */,
				@PersonId = @PersonId,
				@NoteText = @NoteText
		END

		-- Determine if the send-out was changed
		IF ISNULL(@PrevSendOut, '') <> ISNULL(@SendOut, '')
		BEGIN
			-- Create a history record
			SET @NoteText = 'Send-Out was: ' + ISNULL(@PrevSendOut, '') + ' now: ' + ISNULL(@SendOut, '')

			EXEC dbo.OpportunityTransitionInsert @OpportunityId = @OpportunityId,
				@OpportunityTransitionStatusId = 5 /* Send-Out */,
				@PersonId = @PersonId,
				@NoteText = @NoteText
		END
		
		IF(@PersonIdList IS NOT NULL)
		BEGIN
			DELETE op
			FROM OpportunityPersons op
			LEFT JOIN [dbo].[ConvertStringListIntoTable] (@PersonIdList) AS p 
			ON op.OpportunityId = @OpportunityId AND op.PersonId = p.ResultId
			WHERE p.ResultId IS NULL and OP.OpportunityId = @OpportunityId

			INSERT INTO OpportunityPersons
			SELECT @OpportunityId ,P.ResultId
			FROM [dbo].[ConvertStringListIntoTable] (@PersonIdList) AS p 
			LEFT JOIN OpportunityPersons op
			ON p.ResultId = op.PersonId AND op.OpportunityId=@OpportunityId
			WHERE op.PersonId IS NULL 
		END
END

