CREATE PROCEDURE [dbo].[IsPersonHaveActiveStatusDuringThisPeriod]
(
	@PersonId           INT,
	@StartDate          DATETIME,
	@EndDate            DATETIME = NULL
)	
AS
BEGIN

	DECLARE @IsPersonHasActiveStatus BIT
	DECLARE @FutureDate DATETIME , @MinStartDate DATETIME ,@NOW DATETIME,@MinStartDateStatusId INT,@PersonHireDate DATETIME
	
	
	SELECT @MinStartDate = MIN(PSH.StartDate) FROM dbo.PersonStatusHistory PSH
					   WHERE PSH.PersonId = @PersonId

	SELECT @MinStartDateStatusId = PSH.PersonStatusId FROM dbo.PersonStatusHistory PSH
					   WHERE PSH.PersonId = @PersonId AND PSH.StartDate = @MinStartDate

	SELECT @PersonHireDate = P.HireDate FROM dbo.Person P WHERE P.PersonId = @PersonId

	SET  @FutureDate = dbo.GetFutureDate()
	SET  @NOW = dbo.GettingPMTime(GETUTCDATE())
	
	SET @IsPersonHasActiveStatus = 0
	
	IF EXISTS(SELECT 1 	FROM dbo.PersonStatusHistory PSH
				WHERE PSH.PersonId = @PersonId AND PSH.PersonStatusId = 1 
						AND ( PSH.StartDate <=  ISNULL(@EndDate,@FutureDate) 
						AND ISNULL(PSH.EndDate,@FutureDate) >= @StartDate))
	BEGIN
		SET @IsPersonHasActiveStatus  = 1
	END
	ELSE 
		BEGIN
			IF (@MinStartDateStatusId = 1 
				AND @PersonHireDate < @StartDate  
				AND @PersonHireDate  < @MinStartDate 
				AND @StartDate < @MinStartDate 
				AND @PersonHireDate < ISNULL(@EndDate,@FutureDate))
			BEGIN
					SET @IsPersonHasActiveStatus  = 1
				END
			ELSE
				BEGIN
					SET @IsPersonHasActiveStatus = 0
				END
		END

		
	SELECT @IsPersonHasActiveStatus

END
