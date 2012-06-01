-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-5-2008
-- Updated by:	Sainath C
-- Update Date:	06-01-2012
-- Description:	Lists the days within the specified period
-- =============================================
CREATE PROCEDURE [dbo].[CalendarGet]
    (
      @StartDate DATETIME ,
      @EndDate DATETIME 
    )
AS 
BEGIN
    SET NOCOUNT ON;

    SELECT  cal.Date ,
			cal.DayOff ,
            cal.DayOff AS CompanyDayOff ,
            cal.IsRecurring ,
            cal.RecurringHolidayId ,
            cal.HolidayDescription ,
            cal.RecurringHolidayDate 
    FROM    dbo.Calendar AS cal
    WHERE   cal.Date BETWEEN @StartDate AND @EndDate
    ORDER BY cal.Date

END

