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

	SELECT  @HoursInPeriod = SUM(dbo.PersonProjectedHoursPerDay(ISNULL(pcal.DayOff, cal.DayOff),cal.DayOff,pcal.ActualHours,MPE.HoursPerDay))
	FROM dbo.Milestone AS m
	INNER JOIN dbo.MilestonePerson AS mp on m.MilestoneId = mp.MilestoneId
	INNER JOIN dbo.MilestonePersonEntry AS mpe on mpe.MilestonePersonId = mp.MilestonePersonId	
	INNER JOIN Calendar AS cal ON cal.Date BETWEEN mpe.StartDate AND mpe.EndDate
	LEFT JOIN PersonCalendar AS pcal ON cal.Date = pcal.Date AND pcal.PersonId = mp.PersonId	
	WHERE mp.MilestonePersonId = @MilestonePersonId	
	
	-- Return the result of the function
	RETURN @HoursInPeriod

END

