--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Description:	Removes a person's default actual commision
-- =============================================
CREATE PROCEDURE [dbo].[DefaultComissionDelete]
(
	@PersonId   INT,
	@Type       INT
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @Today DATETIME,
			@PayEndDate DATETIME,
			@PayStartDate DATETIME,
			@IsActivePay BIT
 
	SET @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETDATE())))

	BEGIN TRAN

	-- Delete the commission was set today
	DELETE
	  FROM dbo.DefaultCommission
	 WHERE PersonId = @PersonId
	   AND [Type] = @Type
	   AND StartDate = @Today

	-- Expire previously set commission
	UPDATE dbo.DefaultCommission
	   SET EndDate = @Today
	 WHERE PersonId = @PersonId
	   AND [Type] = @Type
	   AND @Today >= StartDate
	   AND @Today < EndDate
	   IF @Type = 1 
	   BEGIN
		  SELECT @PayEndDate = EndDate,
				 @IsActivePay = [IsActivePay]
		  FROM dbo.Pay
		  WHERE Person = @PersonId AND StartDate < @Today AND EndDate > @Today
				IF(@PayEndDate IS NOT NULL)
				BEGIN
					UPDATE dbo.Pay
					SET EndDate = @Today,
						[IsActivePay] = 0
					WHERE EndDate = @PayEndDate AND Person = @PersonId
			
					INSERT INTO dbo.Pay
					SELECT [Person]
						,@Today
						,@PayEndDate
						,[Amount]
						,[Timescale]
						,[TimesPaidPerMonth]
						,[Terms]
						,[VacationDays]
						,[BonusAmount]
						,[BonusHoursToCollect]
						,[DefaultHoursPerDay]
						,[SeniorityId]
						,[PracticeId]
						,@IsActivePay
					FROM [dbo].[Pay]
					WHERE Person = @PersonId AND EndDate = @Today	
				END
		END
	COMMIT TRAN

