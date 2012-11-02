CREATE PROCEDURE [dbo].[GetWorkTypeById]
(
	@TimeTypeId INT
)
AS
BEGIN
	DECLARE	@ORTTimeTypeId	INT = dbo.GetORTTimeTypeId(),
			@UnpaidTimeTypeId	INT = dbo.GetUnpaidTimeTypeId()

	SELECT	TT.TimeTypeId,
			TT.Name,
			CASE WHEN TT.TimeTypeId = @ORTTimeTypeId THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END [IsORTTimeType],
			CASE WHEN TT.TimeTypeId = @UnpaidTimeTypeId THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END [IsUnpaidTimeType],
			TT.IsW2HourlyAllowed,
			TT.IsW2SalaryAllowed
	 FROM dbo.TimeType TT
	 WHERE TimeTypeId = @TimeTypeId
END

