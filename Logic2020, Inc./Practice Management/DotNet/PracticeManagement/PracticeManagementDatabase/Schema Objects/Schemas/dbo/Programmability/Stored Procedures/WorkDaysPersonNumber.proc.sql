--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-18-2008
-- Updated by:  
-- Update date: 
-- Description:	Gets a number of work days for the period
-- =============================================
CREATE PROCEDURE [dbo].[WorkDaysPersonNumber]
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON

	SELECT COUNT(*) WorkDays
	  FROM dbo.v_PersonCalendar AS cal
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	   AND cal.DayOff = 0
	   AND cal.PersonId = @PersonId

