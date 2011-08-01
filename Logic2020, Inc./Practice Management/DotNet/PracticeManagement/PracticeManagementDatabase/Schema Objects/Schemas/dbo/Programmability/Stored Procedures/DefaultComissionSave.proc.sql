--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Description:	Saves a person's default actual commisions
-- =============================================
CREATE PROCEDURE [dbo].[DefaultComissionSave]
(
	@PersonId           INT,
	@FractionOfMargin   DECIMAL(18,2),
	@Type               INT,
	@MarginTypeId       INT
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

	IF EXISTS(SELECT 1
	            FROM dbo.[DefaultCommission] AS c
	           WHERE c.PersonId = @PersonId
	             AND c.[Type] = @Type
	             AND c.StartDate = @Today)
	BEGIN
		-- The value was already changed today
		UPDATE dbo.[DefaultCommission]
		   SET FractionOfMargin = @FractionOfMargin,
		       MarginTypeId = @MarginTypeId
		 WHERE PersonId = @PersonId
		   AND [Type] = @Type
		   AND StartDate = @Today
	END
	ELSE
	BEGIN
		-- There is no value set today

		DECLARE @OldFractionOfMargin DECIMAL(18,2),
				@OldMarginTypeId INT
		
		SELECT @OldFractionOfMargin = FractionOfMargin,
			 @OldMarginTypeId = MarginTypeId
		FROM dbo.[DefaultCommission]
		WHERE PersonId = @PersonId
		   AND [Type] = @Type
		   AND @Today >= StartDate
		   AND @Today < EndDate
		IF(@OldFractionOfMargin <> @FractionOfMargin  
		  OR @OldFractionOfMargin IS NULL AND @FractionOfMargin IS NOT NULL
		  OR @OldFractionOfMargin IS NOT NULL AND @FractionOfMargin IS NULL
		  OR @OldMarginTypeId <> @MarginTypeId
		  OR @OldMarginTypeId IS NULL AND @MarginTypeId IS NOT NULL
		  OR @OldMarginTypeId IS NOT NULL AND @MarginTypeId IS NULL)
		BEGIN
			-- Expire old data
			UPDATE dbo.[DefaultCommission]
			   SET EndDate = @Today
			 WHERE PersonId = @PersonId
			   AND [Type] = @Type
			   AND @Today >= StartDate
			   AND @Today < EndDate

			DECLARE @MaxPayEndDate DATETIME
			IF(@Type = 1)
			BEGIN
				SELECT @MaxPayEndDate = MAX(EndDate)
				FROM dbo.Pay
				WHERE Person = @PersonId

				SELECT @PayEndDate = EndDate,
						@IsActivePay = [IsActivePay]
				FROM dbo.Pay
				WHERE Person = @PersonId AND StartDate < @Today AND EndDate > @Today
			END

			-- Set new comission
			INSERT INTO dbo.[DefaultCommission]
						(PersonId, StartDate, EndDate, FractionOfMargin, [type], MarginTypeId)
			VALUES (@PersonId, @Today, CASE WHEN @MaxPayEndDate>@PayEndDate AND @Type = 1 THEN @PayEndDate ELSE '2029-12-31' END, 
					@FractionOfMargin, @Type, @MarginTypeId)

			IF(@PayEndDate IS NOT NULL AND @Type = 1)
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
	END

	COMMIT

