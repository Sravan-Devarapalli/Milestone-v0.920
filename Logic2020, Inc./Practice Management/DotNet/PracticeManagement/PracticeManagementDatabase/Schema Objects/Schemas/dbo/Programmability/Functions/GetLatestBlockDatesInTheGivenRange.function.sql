CREATE FUNCTION [dbo].[GetLatestBlockDatesInTheGivenRange]
(
	@Startdate	DATETIME, 
	@Enddate	DATETIME
)
RETURNS TABLE
AS
RETURN
(
	With BlockDates
	AS
	(
		SELECT M.PersonId,M.OrganicBreakStartDate AS BlockStartDate,M.OrganicBreakEndDate AS BlockEndDate
		FROM MSBadge M
		UNION 
		SELECT M.PersonId,M.BlockStartDate,M.BlockEndDate
		FROM MSBadge M
		UNION 
		SELECT MS.PersonId,MS.BreakStartDate,MS.BreakEndDate
		FROM MSBadge M
		LEFT JOIN v_CurrentMSBadge MS ON MS.PersonId = M.PersonId
	),
	PersonBlockDate
	AS
	(
		SELECT PersonId, BlockStartDate, BlockEndDate, ROW_NUMBER() OVER(PARTITION BY PersonId ORDER BY BlockEndDate DESC) AS RNo
		FROM BlockDates
		WHERE BlockStartDate IS NOT NULL AND BlockEndDate IS NOT NULL
	)
	SELECT PB.PersonId, PB.BlockStartDate, PB.BlockEndDate
	FROM PersonBlockDate PB
	WHERE PB.RNo = 1
)
