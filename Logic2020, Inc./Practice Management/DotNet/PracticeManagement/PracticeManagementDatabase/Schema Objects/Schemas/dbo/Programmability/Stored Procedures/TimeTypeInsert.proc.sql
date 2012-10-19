-- =============================================
-- Author:		ThulasiRam.P
-- Modified date: 2012-02-14
-- Last Updated date:2012-04-20
-- Description:	Insert new time type
-- =============================================
CREATE PROCEDURE [dbo].[TimeTypeInsert]
(
	@TimeTypeId   INT OUT,
	@Name VARCHAR(50),
	@IsDefault BIT,
	@IsInternal	BIT,
	@IsActive	BIT,
	@IsAdministrative BIT
)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
			BEGIN TRANSACTION TimeTypeInsert
			
			DECLARE @Error NVARCHAR(200)

			IF EXISTS(SELECT 1 FROM dbo.TimeType WHERE [Name] = @Name)
			BEGIN
				
				SET @Error = 'Work Type with this name already exists. Please enter a different Work Type name.'
				RAISERROR(@Error,16,1)
				RETURN
			END
			/*
			@IsDefault = 0 for Custom WorkType  W0000 - W5999.
						= 1 for Default WorkType. W6000 - W6999.
			 */
			 DECLARE	 @TimeTypeCode			 NVARCHAR(10),
						 @LowerLimitRange		 INT ,
						 @HigherLimitRange		 INT ,
						 @NextTimeTypeNumber	 INT

			IF (@IsInternal = 1)
			BEGIN
				SET @LowerLimitRange = 7000
				SET @HigherLimitRange = 9999
				SET @Error = 'Internal worktype code not avaliable'
			END
			ELSE IF (@IsDefault = 1)
			BEGIN
				SET @LowerLimitRange = 6000
				SET @HigherLimitRange = 6999
				SET @Error = 'Default worktype code not avaliable'
			END
			ELSE
			BEGIN
				SET @LowerLimitRange = 0
				SET @HigherLimitRange = 5999
				SET @Error = 'Custom worktype code not avaliable'
			END

			DECLARE @TimeTypeRanksList TABLE (TimeTypeNumber INT,TimeTypeNumberRank INT)
			INSERT INTO @TimeTypeRanksList 
			SELECT Convert(INT,SUBSTRING(Code,2,5)) as TimeTypeNumber ,
					RANK() OVER (ORDER BY CONVERT(INT,SUBSTRING(Code,2,5))) + @LowerLimitRange - 1 AS  TimeTypeNumberRank
			FROM dbo.TimeType AS TT
			WHERE TT.IsDefault = @IsDefault AND TT.IsInternal = @IsInternal AND ISNUMERIC( SUBSTRING(Code,2,5)) = 1
			AND Convert(INT,SUBSTRING(Code,2,5)) BETWEEN @LowerLimitRange AND @HigherLimitRange


			INSERT INTO @TimeTypeRanksList 
			SELECT -1, MAX(TimeTypeNumberRank)+1 FROM @TimeTypeRanksList

			SELECT TOP 1 @NextTimeTypeNumber = TimeTypeNumberRank 
			FROM @TimeTypeRanksList  
			WHERE TimeTypeNumber != TimeTypeNumberRank 
			ORDER BY TimeTypeNumberRank

				
			IF (@NextTimeTypeNumber IS NULL AND NOT EXISTS(SELECT 1 FROM @TimeTypeRanksList WHERE TimeTypeNumber != -1))
			BEGIN 
				SET  @NextTimeTypeNumber = @LowerLimitRange
			END 
			ELSE IF (@NextTimeTypeNumber > @HigherLimitRange )
			BEGIN
				RAISERROR (@Error, 16, 1)
				RETURN
			END

			SET @TimeTypeCode = 'W'+ REPLICATE('0',4 - LEN(@NextTimeTypeNumber)) + CONVERT(NVARCHAR,@NextTimeTypeNumber)

			INSERT INTO dbo.TimeType ([Name], [IsDefault], [IsInternal], [IsAllowedToEdit],Code,[IsActive],IsAdministrative) 
			VALUES (@Name, @IsDefault, @IsInternal, 1,@TimeTypeCode,@IsActive,@IsAdministrative)

			SET @TimeTypeId = SCOPE_IDENTITY()

			--If default timetype is inserted, need to unassign it for Internal projects and for 'Business Development' project.
			IF (@IsDefault = 1)
			BEGIN
				INSERT INTO dbo.ProjectTimeType(ProjectId,TimeTypeId,IsAllowedToShow)
				SELECT P.ProjectId, @TimeTypeId, 0
				FROM dbo.Project AS P
				WHERE P.IsInternal = 1 OR P.ProjectNumber = 'P999918'--Here 'P999918' is Business Development project's ProjectNumber(Code).
		
			END
			COMMIT TRANSACTION TimeTypeInsert
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION TimeTypeInsert

		SET @Error = ERROR_MESSAGE()

		RAISERROR(@Error, 16, 1)

	END CATCH

END

