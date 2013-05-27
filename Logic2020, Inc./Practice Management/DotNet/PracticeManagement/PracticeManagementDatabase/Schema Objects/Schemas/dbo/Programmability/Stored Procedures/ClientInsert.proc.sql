CREATE PROCEDURE dbo.ClientInsert
(
	@Name                 NVARCHAR(50),
	@DefaultDiscount      DECIMAL(18,2),
	@DefaultSalespersonId INT, 
	@DefaultDirectorId	  INT,
	@Inactive             BIT,
	@IsChargeable         BIT,
	@DefaultTerms         INT,
	@ClientId             INT OUTPUT,
	@IsMarginColorInfoEnabled BIT = NULL,
	@IsInternal BIT,
	@IsNoteRequired     BIT = 1,
	@IsHouseAccount BIT,
	@UserLogin          NVARCHAR(255)
)
AS
	SET NOCOUNT ON
	
	DECLARE @ErrorMessage NVARCHAR(2048)
	IF EXISTS (SELECT 1 FROM dbo.Client AS c WHERE c.Name = @Name)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70004)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN
		BEGIN TRY
		
		/*
		NOTE:At present there is no separate range specified for internal or external clients and For Logic2020 code: C2020.
		RANGE : C0000 - C9999
		*/

		DECLARE  @ClientCode			 NVARCHAR(10),
				 @LowerLimitRange		 INT ,
				 @HigherLimitRange		 INT ,
				 @NextClientNumber		 INT,
				 @Error					 NVARCHAR(MAX)
		SET @LowerLimitRange = 0
		SET @HigherLimitRange = 9999
		SET @Error = 'Account code not available'

		DECLARE @ClientRanksList TABLE (ClientNumber INT,ClientNumberRank INT)
		INSERT INTO @ClientRanksList 
		SELECT Convert(INT,SUBSTRING(Code,2,5)) as ClientNumber ,
				RANK() OVER (ORDER BY Convert(INT,SUBSTRING(Code,2,5)))+@LowerLimitRange-1 AS  clientNumberRank
		FROM dbo.Client  
		WHERE ISNUMERIC( SUBSTRING(Code,2,5)) = 1


		INSERT INTO @ClientRanksList 
		SELECT -1,MAX(ClientNumberRank)+1 FROM @ClientRanksList

		SELECT TOP 1 @NextClientNumber = ClientNumberRank 
			FROM @ClientRanksList  
			WHERE ClientNumber != ClientNumberRank 
			ORDER BY ClientNumberRank

				
		IF (@NextClientNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @ClientRanksList WHERE ClientNumber != -1))
		BEGIN 
			SET  @NextClientNumber = @LowerLimitRange
		END 
		ELSE IF (@NextClientNumber > @HigherLimitRange )
		BEGIN
			RAISERROR (@Error, 16, 1)
		END

		SET @ClientCode = 'C'+ REPLICATE('0',4-LEN(@NextClientNumber)) +CONVERT(NVARCHAR,@NextClientNumber)

		-- Start logging session
	    EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		INSERT INTO Client
					(DefaultDiscount, DefaultTerms, DefaultSalespersonId, DefaultDirectorID, Name, Inactive, IsChargeable,IsMarginColorInfoEnabled,IsInternal,Code,IsNoteRequired,IsHouseAccount)
			 VALUES (@DefaultDiscount, @DefaultTerms, @DefaultSalespersonId, @DefaultDirectorId, @Name, @Inactive, @IsChargeable,@IsMarginColorInfoEnabled,@IsInternal,@clientCode,@IsNoteRequired,@IsHouseAccount)

		SELECT @ClientId = SCOPE_IDENTITY()

		SELECT @ClientId

		END TRY
		BEGIN CATCH
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR (@ErrorMessage, 16, 1)
		END CATCH
	END
GO

