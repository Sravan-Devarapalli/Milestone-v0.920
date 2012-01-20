CREATE PROCEDURE [dbo].[SetChargeCodeTurnOffHistory]
	@ClientId            INT,
    @ProjectGroupId      INT,
    @ProjectId           INT,
    @PhaseId             INT,
    @TimeTypeId          INT,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@IsTrunOff			BIT
AS
BEGIN
-- If you turn off Charge code then Put future turnoff date into start date column. And Put Endate To null
-- Again if you turn on then -> if start date is greater than today then simply delete the row 
--			       else Put Endate as Today for existing history row.
--    And above is repated..

	
	DECLARE @ChargeCodeId INT,
			@Today		DATETIME

	SET @Today = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE()))

	SELECT @ChargeCodeId = id
	FROM ChargeCode
	WHERE ClientId = @ClientId
		AND ProjectGroupId = @ProjectGroupId
		AND ProjectId = @ProjectId
		AND PhaseId = @PhaseId
		AND TimeTypeId = @TimeTypeId

	IF @IsTrunOff = 1
	BEGIN
		INSERT INTO ChargeCodeTurnOffHistory(ChargeCodeId, StartDate, EndDate)
		VALUES (@ChargeCodeId, CONVERT(DATE, @StartDate), CONVERT(DATE, @EndDate))
	END
	ELSE
	BEGIN
		--if start date is greater than today then simply delete the entry. 
		DELETE CCTH
		FROM ChargeCodeTurnOffHistory CCTH
		WHERE CCTH.ChargeCodeId = @ChargeCodeId
			AND CCTH.StartDate > @Today
		
		UPDATE CCTH
		SET EndDate = @Today
		FROM ChargeCodeTurnOffHistory CCTH
		WHERE CCTH.ChargeCodeId = @ChargeCodeId
			AND CCTH.EndDate > @Today AND CCTH.StartDate < @Today

	END
END
