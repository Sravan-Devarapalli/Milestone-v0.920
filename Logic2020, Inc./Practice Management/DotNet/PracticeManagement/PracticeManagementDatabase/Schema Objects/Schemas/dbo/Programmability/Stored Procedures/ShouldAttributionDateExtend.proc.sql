CREATE PROCEDURE [dbo].[ShouldAttributionDateExtend]
	(
		@ProjectId		INT,
		@StartDate		DATETIME,--Milestone start date 
		@EndDate		DATETIME --Milestone end date
	)
AS
BEGIN

		DECLARE @ProjectStartDate	DATETIME,
				@ProjectEndDate		DATETIME,
				@IsAttributionEndDateExtend BIT = 0,
				@IsAttributionStartDateExtend BIT = 0

		SELECT	@ProjectStartDate = P.StartDate,
				@ProjectEndDate = P.EndDate
		FROM	dbo.Project P
		WHERE	P.ProjectId = @ProjectId
		--If new milestone range overlapps with old project range  
		IF @StartDate <= @ProjectEndDate AND @ProjectStartDate <= @EndDate OR (@ProjectStartDate IS NOT NULL)
		BEGIN
				IF EXISTS
								(SELECT A.AttributionId
								FROM dbo.Attribution A
								WHERE @StartDate < @ProjectStartDate AND A.ProjectId = @ProjectId AND A.StartDate = @ProjectStartDate)
				BEGIN
						SET @IsAttributionStartDateExtend = 1
				END

				IF EXISTS
								(SELECT A.AttributionId
								FROM dbo.Attribution A
								WHERE @EndDate > @ProjectEndDate AND A.ProjectId = @ProjectId AND A.EndDate = @ProjectEndDate)
				BEGIN
						SET @IsAttributionEndDateExtend = 1
				END
		END
		ELSE IF @StartDate > @ProjectEndDate OR @ProjectStartDate > @EndDate
		BEGIN
				SET @IsAttributionStartDateExtend = 1
				SET @IsAttributionEndDateExtend = 1
		END
		
		SELECT @IsAttributionStartDateExtend AS ExtendAttributionStartDate,@IsAttributionEndDateExtend AS ExtendAttributionEndDate
END
