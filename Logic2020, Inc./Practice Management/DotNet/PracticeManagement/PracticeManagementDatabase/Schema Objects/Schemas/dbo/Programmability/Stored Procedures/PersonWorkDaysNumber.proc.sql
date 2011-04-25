-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-21-2008
-- Updated by:	
-- Update date:	
-- Description:	Retrieves a number of work days for the specified person and period
-- =============================================
CREATE PROCEDURE [dbo].[PersonWorkDaysNumber]
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON

	SELECT COUNT(*) AS WorkDays
	  FROM dbo.v_PersonCalendar AS c
	 WHERE c.PersonId = @PersonId
	   AND c.Date BETWEEN @StartDate AND @EndDate
	   AND c.DayOff = 0

