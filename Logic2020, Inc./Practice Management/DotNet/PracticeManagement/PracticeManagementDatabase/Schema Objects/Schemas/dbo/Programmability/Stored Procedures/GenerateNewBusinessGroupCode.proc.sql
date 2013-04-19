CREATE PROCEDURE [dbo].[GenerateNewBusinessGroupCode]
	@IsInternal BIT, 
	@BusinessGroupCode NVARCHAR (6) OUTPUT
AS
BEGIN
	/*
		For Business Group
		internal Business Group range : B9900-B9999 
		external Business Group range : B0000-B9899
		*/
		DECLARE @ErrorMessage	NVARCHAR(MAX)

		BEGIN TRY

		DECLARE  @LowerLimitRange		 INT ,
				 @HigherLimitRange		 INT ,
				 @NextBusinessGroupNumber INT

		IF (@IsInternal = 1)
		BEGIN
			SET @LowerLimitRange = 9900
			SET @HigherLimitRange = 9999
			SET @ErrorMessage = 'Internal business Group code not available'
		END
		ELSE
		BEGIN
			SET @LowerLimitRange = 0
			SET @HigherLimitRange = 9899
			SET @ErrorMessage = 'External business Group code not available'
		END

		DECLARE @BusinessGroupRanksList TABLE (businessGroupNumber INT,businessGroupNumberRank INT)
		INSERT INTO @BusinessGroupRanksList 
		SELECT Convert(INT,SUBSTRING(pg.Code,2,5)) as BusinessGroupNumber ,
				RANK() OVER (ORDER BY Convert(INT,SUBSTRING(pg.Code,2,5)))+@LowerLimitRange-1 AS  BusinessGroupNumberRank
		FROM dbo.BusinessGroup pg INNER JOIN dbo.Client c ON c.ClientId = pg.ClientId AND c.IsInternal = @IsInternal 
		WHERE ISNUMERIC( SUBSTRING(pg.Code,2,5)) = 1


		INSERT INTO @BusinessGroupRanksList 
		SELECT -1,MAX(BusinessGroupNumberRank)+1 FROM @BusinessGroupRanksList

		SELECT TOP 1 @NextBusinessGroupNumber = businessGroupNumber 
			FROM @BusinessGroupRanksList  
			WHERE businessGroupNumber != businessGroupNumberRank 
			ORDER BY businessGroupNumberRank

				
		IF (@NextBusinessGroupNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @BusinessGroupRanksList WHERE businessGroupNumber != -1))
		BEGIN 
			SET  @NextBusinessGroupNumber = @LowerLimitRange
		END 
		ELSE IF (@NextBusinessGroupNumber > @HigherLimitRange )
		BEGIN
			RAISERROR (@ErrorMessage, 16, 1)
		END

		SET @BusinessGroupCode = 'BG'+ REPLICATE('0',4-LEN(@NextBusinessGroupNumber)) + CONVERT(NVARCHAR,@NextBusinessGroupNumber)

		END TRY
		BEGIN CATCH
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR (@ErrorMessage, 16, 1)
		END CATCH
END
