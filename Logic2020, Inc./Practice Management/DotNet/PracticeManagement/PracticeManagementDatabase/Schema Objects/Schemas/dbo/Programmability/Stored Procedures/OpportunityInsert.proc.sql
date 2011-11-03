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
	@OwnerId	           INT = NULL,
	@GroupId	           INT,
	@EstimatedRevenue      DECIMAL(18,2) ,
	@PersonIdList          NVARCHAR(MAX) = NULL,
	@StrawManList          NVARCHAR(MAX) = NULL
)
AS
BEGIN
	    SET NOCOUNT ON;
		SET ANSI_NULLS ON;
		SET  QUOTED_IDENTIFIER ON;
		
		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

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


		IF(@PersonIdList IS NOT NULL)
		BEGIN
			INSERT INTO OpportunityPersons(OpportunityId,PersonId,OpportunityPersonTypeId,RelationTypeId)
			SELECT @OpportunityId ,P.ResultId,P.ResultType,1-- Relation type 1 means PropesedResource
			FROM dbo.[ConvertStringListIntoTableWithTwoColoumns] (@PersonIdList) AS p 
			LEFT JOIN OpportunityPersons op
			ON p.ResultId = op.PersonId AND op.OpportunityId=@OpportunityId
			WHERE op.PersonId IS NULL 
		END

		IF(@StrawManList IS NOT NULL  AND ISNULL(@StrawManList,'')<>'')
		BEGIN

			DECLARE @OpportunityPersonIdsWithTypeTable TABLE
			(
			PersonId INT,
			PersonType INT,
			Quantity INT
			)

			DECLARE @PersonIdListLocalXML XML
			IF(SUBSTRING(@StrawManList,LEN(@StrawManList),1)=',')
			SET @StrawManList = SUBSTRING(@StrawManList,1,LEN(@StrawManList)-1)
			SET @StrawManList = '<root><item><personid>'+@StrawManList+'</qty></item></root>'

			SET @StrawManList = REPLACE(@StrawManList,':','</personid><persontypeid>')
			SET @StrawManList = REPLACE(@StrawManList,'|','</persontypeid><qty>')
			SET @StrawManList = REPLACE(@StrawManList,',','</qty></item><item><personid>')
			
			SELECT @StrawManList
			SELECT @PersonIdListLocalXML  = CONVERT(XML,@StrawManList)

			INSERT INTO @OpportunityPersonIdsWithTypeTable
			(PersonId ,
			PersonType ,
			Quantity )
			SELECT C.value('personid[1]','int') personid,
					C.value('persontypeid[1]','int') persontypeid,
					C.value('qty[1]','int') qty
			FROM @PersonIdListLocalXML.nodes('/root/item') as T(C)

			INSERT INTO OpportunityPersons(OpportunityId,PersonId,OpportunityPersonTypeId,RelationTypeId,Quantity)
			SELECT @OpportunityId ,p.PersonId,p.PersonType,2,p.Quantity
			FROM @OpportunityPersonIdsWithTypeTable AS p 
			LEFT JOIN dbo.OpportunityPersons op
			ON p.PersonId = op.PersonId AND op.OpportunityId=@OpportunityId AND op.OpportunityPersonTypeId=p.PersonType
			WHERE op.PersonId IS NULL 

		END

		-- End logging session
		EXEC dbo.SessionLogUnprepare
	
END

