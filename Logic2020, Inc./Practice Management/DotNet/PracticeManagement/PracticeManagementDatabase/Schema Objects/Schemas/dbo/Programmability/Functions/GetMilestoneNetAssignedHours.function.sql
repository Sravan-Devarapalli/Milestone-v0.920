-- =============================================
-- Author:		Thulasiram
-- Create date: 07-13-2012
-- Update by:	
-- Update date:	
-- Description:	Gives the sum of person work hours in the milestone without vacation days.
-- =============================================
CREATE FUNCTION GetMilestoneNetAssignedHours(@MilestoneId INT)
RETURNS @Result TABLE 
(
	TotalHours DECIMAL(15,2)
)
AS
BEGIN
	
	INSERT INTO @Result
	SELECT SUM(MPE.HoursPerDay)
	FROM   dbo.MilestonePerson AS MP  
	INNER JOIN dbo.MilestonePersonEntry AS  MPE ON MP.MileStonePersonId = MPE.MileStonePersonId
	INNER JOIN dbo.PersonCalendarAuto AS pcal 	ON pcal.Date BETWEEN mpe.Startdate AND mpe.EndDate 
										   AND pcal.DayOff=0 AND pcal.PersonId = mp.PersonId 
	WHERE  mp.MileStoneId = @MilestoneId
	GROUP BY MP.MilestoneId

	RETURN
END

GO
