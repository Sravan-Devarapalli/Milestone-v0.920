-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-08-04
-- Description:	Get number of hours in period for specified milestone person
-- =============================================
CREATE FUNCTION GetMilestonePersonHoursInPeriod 
(
	-- Add the parameters for the function here
	@MilestonePersonId int
)
RETURNS decimal(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @HoursInPeriod decimal(18,2)

	select @HoursInPeriod = SUM(mp.HoursPerDay * mp.VacationDays + mp.ExpectedHours)
	from v_MilestonePerson mp 
	where mp.MilestonePersonId=@MilestonePersonId
	
	-- Return the result of the function
	RETURN @HoursInPeriod

END

