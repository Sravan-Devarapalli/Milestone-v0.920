-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-05-07
-- Description:	If given date range is withing given time interval
-- =============================================
CREATE FUNCTION dbo.IsDateRangeWithingTimeInterval 
(
	@IntervalStart datetime,
	@IntervalEnd datetime,
	@RangeStart datetime,
	@RangeEnd datetime
)
RETURNS bit
AS
BEGIN

	-- When interval start date is between range start and end dates
	if datediff(day, @RangeStart, @IntervalStart) >= 0 AND
		datediff(day, @IntervalStart, @RangeEnd) >= 0
		return 1

	-- When interval end date is between range start and end dates
	if datediff(day, @RangeStart, @IntervalEnd) >= 0 AND
		datediff(day, @IntervalEnd, @RangeEnd) >= 0
		return 1

	-- When interval is within range
	if datediff(day, @RangeStart, @IntervalStart) >= 0 AND
		datediff(day, @IntervalEnd, @RangeEnd) >= 0
		return 1

	-- When range is within interval
	if datediff(day, @IntervalStart, @RangeStart) >= 0 AND
		datediff(day, @RangeEnd, @IntervalEnd) >= 0
		return 1


	return 0

END

