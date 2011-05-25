CREATE PROCEDURE dbo.UpdateMinimumLoadFactorHistory
(
	@TimescaleId int ,
	@Rate decimal(18, 5) 
)
AS
BEGIN
	DECLARE @Today DATETIME,
	@PrevStartDate DATETIME,
	@PrevRate	decimal(18, 5),
	@OverheadFixedRateId  INT
	
	SELECT @Today  = CONVERT(DATETIME,CONVERT(DATE,GETDATE()))
	
	SELECT @OverheadFixedRateId  = OverheadFixedRateId
	FROM dbo.OverheadFixedRate
	WHERE IsMinimumLoadFactor = 1

	SELECT @PrevStartDate = [StartDate],
			@PrevRate =[Rate]
	FROM dbo.MinimumLoadFactorHistory 
	WHERE [OverheadFixedRateId]= @OverheadFixedRateId  
		AND TimescaleId = @TimescaleId
		AND EndDate Is NULL
		 
	IF ( @PrevRate <> @Rate)
	BEGIN
	
		IF(@PrevStartDate = @Today)
		BEGIN
			DECLARE @RateTillYesterDay decimal(18, 5)

			SELECT @RateTillYesterDay = Rate
			FROM dbo.MinimumLoadFactorHistory 
			WHERE [OverheadFixedRateId]= @OverheadFixedRateId  
					AND TimescaleId = @TimescaleId
					AND EndDate = @Today-1
			
			IF(@RateTillYesterDay = @Rate)
			BEGIN
				UPDATE dbo.MinimumLoadFactorHistory 
				SET EndDate = NULL
				WHERE 	[OverheadFixedRateId]= @OverheadFixedRateId  
						AND TimescaleId = @TimescaleId
						AND EndDate = @Today-1

				DELETE FROM dbo.MinimumLoadFactorHistory 
				WHERE [OverheadFixedRateId]= @OverheadFixedRateId  
						AND TimescaleId = @TimescaleId
						AND [StartDate] = @Today 
			END
			ELSE
			BEGIN
				UPDATE dbo.MinimumLoadFactorHistory 
				SET [Rate] = @Rate
				WHERE [OverheadFixedRateId]= @OverheadFixedRateId  
						AND TimescaleId = @TimescaleId
						AND EndDate Is NULL
			END
			
		END
		ELSE
		BEGIN
			UPDATE dbo.MinimumLoadFactorHistory 
			SET EndDate = @Today -1
			WHERE [OverheadFixedRateId]= @OverheadFixedRateId  
					AND TimescaleId = @TimescaleId
					AND EndDate Is NULL
					
			INSERT INTO [dbo].[MinimumLoadFactorHistory]
				   ([OverheadFixedRateId]
				   ,[TimescaleId]
				   ,[Rate]
				   ,[StartDate]
				   ,[EndDate])
			 VALUES
				   (@OverheadFixedRateId
				   ,@TimescaleId 
				   ,@Rate 
				   ,@Today
				   ,NULL)
		END
	END
END
