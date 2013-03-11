CREATE FUNCTION [dbo].[PersonProjectedHoursPerDay]
(
	@PeronsDayOff BIT, 
	@CompanyDayOff BIT,
	@PersonDayOffHours DECIMAL (4, 2),
	@MilestoneHours DECIMAL (4, 2)
)
RETURNS DECIMAL(4, 2)
AS
BEGIN
	--RETURN CASE WHEN @PeronsDayOff = 1 AND @CompanyDayOff = 0 THEN  --person day off
	--									(CASE WHEN ISNULL(@PersonDayOffHours,0) = 8 THEN 0
	--											ELSE (CASE WHEN ((@PersonDayOffHours/8) * (@MilestoneHours)) > @MilestoneHours THEN 0
	--													ELSE   (@MilestoneHours) - ((@PersonDayOffHours/8) * (@MilestoneHours)) 
	--													END)
	--										END)
	--			WHEN @PeronsDayOff = 0 THEN @MilestoneHours
	--			ELSE 0 --Company holiday
	--		END




	RETURN CASE WHEN @PeronsDayOff = 0  THEN @MilestoneHours -- No Time-off and no company holiday
				WHEN (@PeronsDayOff = 1 and @CompanyDayOff = 1) OR (@PeronsDayOff = 1 AND @CompanyDayOff = 0 AND ISNULL(@PersonDayOffHours,8) = 8) THEN 0 -- only company holiday OR person complete dayoff
				ELSE @MilestoneHours * (1-(@PersonDayOffHours/8)) --person partial day off
				END

END
