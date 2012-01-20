CREATE PROCEDURE [dbo].[GenerateNewProjectGroupCode]
	@IsInternal BIT, 
	@ProjectGroupCode NVARCHAR (5) OUTPUT
AS
BEGIN
	/*
		For Business Unit i.e. ProjectGroup 
		internal ProjectGroup range : B9900-B9999 
		external ProjectGroup range : B0000-B9899
		NOTE : code for Operations; Bench Management under logic2020 client  is 'B9900' and 'B9950' respectively.
		*/
		DECLARE @ErrorMessage	NVARCHAR(MAX)

		BEGIN TRY

		DECLARE  @LowerLimitRange		 INT ,
				 @HigherLimitRange		 INT ,
				 @NextProjectGroupNumber INT

		IF (@IsInternal = 1)
		BEGIN
			SET @LowerLimitRange = 9900
			SET @HigherLimitRange = 9999
			SET @ErrorMessage = 'Internal business Unit code not avaliable'
		END
		ELSE
		BEGIN
			SET @LowerLimitRange = 0
			SET @HigherLimitRange = 9899
			SET @ErrorMessage = 'External business Unit code not avaliable'
		END

		DECLARE @ProjectGroupRanksList TABLE (projectGroupNumber INT,projectGroupNumberRank INT)
		INSERT INTO @ProjectGroupRanksList 
		SELECT Convert(INT,SUBSTRING(pg.Code,2,5)) as projectGroupNumber ,
				RANK() OVER (ORDER BY Convert(INT,SUBSTRING(pg.Code,2,5)))+@LowerLimitRange-1 AS  projectGroupNumberRank
		FROM dbo.ProjectGroup pg INNER JOIN dbo.Client c ON c.ClientId = pg.ClientId AND c.IsInternal = @IsInternal 
		WHERE ISNUMERIC( SUBSTRING(pg.Code,2,5)) = 1


		INSERT INTO @ProjectGroupRanksList 
		SELECT -1,MAX(projectGroupNumberRank)+1 FROM @ProjectGroupRanksList

		SELECT TOP 1 @NextProjectGroupNumber = projectGroupNumberRank 
			FROM @ProjectGroupRanksList  
			WHERE projectGroupNumber != projectGroupNumberRank 
			ORDER BY projectGroupNumberRank

				
		IF (@NextProjectGroupNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @ProjectGroupRanksList WHERE projectGroupNumber != -1))
		BEGIN 
			SET  @NextProjectGroupNumber = @LowerLimitRange
		END 
		ELSE IF (@NextProjectGroupNumber > @HigherLimitRange )
		BEGIN
			RAISERROR (@ErrorMessage, 16, 1)
		END

		SET @ProjectGroupCode = 'B'+ REPLICATE('0',4-LEN(@NextProjectGroupNumber)) + CONVERT(NVARCHAR,@NextProjectGroupNumber)

		END TRY
		BEGIN CATCH
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR (@ErrorMessage, 16, 1)
		END CATCH
END
