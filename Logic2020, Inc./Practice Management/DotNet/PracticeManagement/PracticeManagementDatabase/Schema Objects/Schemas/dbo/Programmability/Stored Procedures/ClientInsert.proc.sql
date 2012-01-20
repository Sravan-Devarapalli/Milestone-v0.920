-- =============================================
-- Author:		Skip Sailors
-- Create date: 4-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-22-2008
-- Description:	Insert a new Client, output and select ID of new client record.
-- =============================================
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
	@IsInternal BIT
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
		NOTE:At present there is no seprate range specified for internal or external clients and For Logic2020 code: C2020.
		RANGE : C0000 - C9999
		*/

		DECLARE  @clientCode			 NVARCHAR(10),
				 @LowerLimitRange		 INT ,
				 @HigherLimitRange		 INT ,
				 @NextClientNumber		 INT,
				 @Error					 NVARCHAR(MAX)
		SET @LowerLimitRange = 0
		SET @HigherLimitRange = 9999
		SET @Error = 'Account code not avaliable'

		DECLARE @ClientRanksList TABLE (clientNumber INT,clientNumberRank INT)
		INSERT INTO @ClientRanksList 
		SELECT Convert(INT,SUBSTRING(Code,2,5)) as clientNumber ,
				RANK() OVER (ORDER BY Convert(INT,SUBSTRING(Code,2,5)))+@LowerLimitRange-1 AS  clientNumberRank
		FROM dbo.Client  
		WHERE ISNUMERIC( SUBSTRING(Code,2,5)) = 1


		INSERT INTO @ClientRanksList 
		SELECT -1,MAX(clientNumberRank)+1 FROM @ClientRanksList

		SELECT TOP 1 @NextClientNumber = clientNumberRank 
			FROM @ClientRanksList  
			WHERE clientNumber != clientNumberRank 
			ORDER BY clientNumberRank

				
		IF (@NextClientNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @ClientRanksList WHERE clientNumber != -1))
		BEGIN 
			SET  @NextClientNumber = @LowerLimitRange
		END 
		ELSE IF (@NextClientNumber > @HigherLimitRange )
		BEGIN
			RAISERROR (@Error, 16, 1)
		END

		SET @clientCode = 'C'+ REPLICATE('0',4-LEN(@NextClientNumber)) +CONVERT(NVARCHAR,@NextClientNumber)

		INSERT INTO Client
					(DefaultDiscount, DefaultTerms, DefaultSalespersonId, DefaultDirectorID, Name, Inactive, IsChargeable,IsMarginColorInfoEnabled,IsInternal,Code)
			 VALUES (@DefaultDiscount, @DefaultTerms, @DefaultSalespersonId, @DefaultDirectorId, @Name, @Inactive, @IsChargeable,@IsMarginColorInfoEnabled,@IsInternal,@clientCode)

		SELECT @clientId = SCOPE_IDENTITY()

		SELECT @clientId

		END TRY
		BEGIN CATCH
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR (@ErrorMessage, 16, 1)
		END CATCH
	END
GO

