--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-18-2008
-- Updated by:  
-- Update date: 
-- Description:	Gets a number of work days for the period
-- =============================================
CREATE PROCEDURE [dbo].[WorkDaysCompanyNumber]
(
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON

	SELECT COUNT(*) WorkDays
	  FROM dbo.Calendar AS cal
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	   AND cal.DayOff = 0

