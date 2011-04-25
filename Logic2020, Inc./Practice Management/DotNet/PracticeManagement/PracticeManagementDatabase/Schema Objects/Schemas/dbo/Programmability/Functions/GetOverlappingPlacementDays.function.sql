-- =============================================
-- Author:		Alexey Zvekov
-- Create date: 8-5-2008
-- Updated by:  
-- Update date: 
-- Description:	Creates a date from its parts.
-- =============================================
CREATE FUNCTION [dbo].[GetOverlappingPlacementDays]
(
	@person_SD		DATETIME, 
	@person_ED		DATETIME, 
	@mileStone_SD	DATETIME, 
	@mileStone_ED	DATETIME
)
RETURNS BIT
AS
/*
Examples:
                                       person_SD    person_ED   mileStone_SD mileStone_ED   result
select dbo.GetOverlappingPlacementDays('20080501', '20080503', '20080505', '20080515')      --0
select dbo.GetOverlappingPlacementDays('20080506', '20080508', '20080505', '20080515')      --1
select dbo.GetOverlappingPlacementDays('20080518', '20080522', '20080505', '20080515')      --0
select dbo.GetOverlappingPlacementDays('20080514', '20080522', '20080505', '20080515')      --1
select dbo.GetOverlappingPlacementDays('20080515', '20080522', '20080505', '20080515')      --1
*/
BEGIN
DECLARE @ReturnValue BIT

SELECT @ReturnValue = CASE
    WHEN @mileStone_ED < @person_SD or @person_ED < @mileStone_SD then 0
    WHEN (datediff(day,
               CASE WHEN @person_SD <= @mileStone_SD THEN @mileStone_SD ELSE @person_SD END,
               CASE WHEN @person_ED <= @mileStone_ED THEN @person_ED ELSE @mileStone_ED END)) > -1 THEN 1
    ELSE null
    END
RETURN @ReturnValue
END

