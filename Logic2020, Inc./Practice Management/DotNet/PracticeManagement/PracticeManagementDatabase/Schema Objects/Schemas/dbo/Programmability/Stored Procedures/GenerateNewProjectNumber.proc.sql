CREATE PROCEDURE dbo.GenerateNewProjectNumber 
(
	@IsInternalProject BIT ,
	@ProjectNumber AS NVARCHAR (12) OUTPUT
)
AS
BEGIN
	/*
		@IsInternal = 1 for internal project. P999900-P999999 
					= 0 for external project. P000000-P999899
	*/
	DECLARE @ErrorMessage NVARCHAR(MAX)
	
	BEGIN TRY

	DECLARE @LowerLimitRange INT ,@HigherLimitRange INT ,@NextProjectNumber INT
	IF (@IsInternalProject = 1)
	BEGIN
		SET @LowerLimitRange = 999900
		SET @HigherLimitRange = 999999
		SET @ErrorMessage = 'Internal project code not avaliable'
	END
	ELSE
	BEGIN
		SET @LowerLimitRange = 0
		SET @HigherLimitRange = 999899
		SET @ErrorMessage = 'External project code not avaliable'
	END

	DECLARE @ProjectRanksList TABLE (projectNumber INT,projectNumberRank INT)
	INSERT INTO @ProjectRanksList 
	SELECT Convert(INT,SUBSTRING(p.ProjectNumber,2,6)) as projectNumber ,
		   RANK() OVER (ORDER BY Convert(INT,SUBSTRING(p.ProjectNumber,2,6)))+@LowerLimitRange-1 AS  projectNumberRank
		FROM dbo.Project p 
		WHERE LEN(p.ProjectNumber) = 7
		AND	 ISNUMERIC( SUBSTRING(ProjectNumber,2,6))  = 1 
		--AND p.IsInternal = @IsInternalProject 
		AND CONVERT(NUMERIC,SUBSTRING(ProjectNumber,2,6)) BETWEEN  @LowerLimitRange AND @HigherLimitRange

	INSERT INTO @ProjectRanksList 
	SELECT -1,MAX(projectNumberRank)+1 FROM @ProjectRanksList 

	SELECT TOP 1 @NextProjectNumber = projectNumberRank 
		FROM @ProjectRanksList  
		WHERE projectNumber != projectNumberRank 
		ORDER BY projectNumberRank

				
	IF (@NextProjectNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @ProjectRanksList WHERE projectNumber != -1))
	BEGIN 
		SET  @NextProjectNumber = @LowerLimitRange
	END 
	ELSE IF (@NextProjectNumber > @HigherLimitRange )
	BEGIN
		RAISERROR (@ErrorMessage, 16, 1)
	END

	SET @ProjectNumber = 'P'+ REPLICATE('0',6-LEN(@NextProjectNumber)) + CONVERT(NVARCHAR,@NextProjectNumber)

	END TRY
	BEGIN CATCH
		SELECT @ErrorMessage = ERROR_MESSAGE()
			
		RAISERROR (@ErrorMessage, 16, 1)
	END CATCH
END


