-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-5-2008
-- Updated by:	Anatoliy Lokshin
-- Update Date:	11-20-2008
-- Description:	Lists the days within the specified period
-- =============================================
CREATE PROCEDURE [dbo].[CalendarGet]
(
	@StartDate           DATETIME,
	@EndDate             DATETIME,
	@PersonId            INT,
	@PracticeManagerId   INT
)
AS
	SET NOCOUNT ON
	
	SELECT cal.Date, cal.DayOff, CAST(NULL AS INT) AS PersonId,
	       cal.DayOff AS CompanyDayOff,
	       CAST(0 AS BIT) AS [ReadOnly]
	  FROM dbo.Calendar AS cal
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	   AND @PersonId IS NULL
	   AND @PracticeManagerId IS NULL
	UNION ALL
	SELECT cal.Date, ISNULL(pcal.DayOff, cal.DayOff) AS DayOff, @PersonId AS PersonId,
	       ISNULL(pcal.CompanyDayOff, cal.DayOff) AS CompanyDayOff,
	       CAST(CASE WHEN pcal.Date IS NULL THEN 1 ELSE 0 END AS BIT) AS [ReadOnly]
	  FROM dbo.Calendar AS cal
	       LEFT JOIN dbo.v_PersonCalendar AS pcal ON cal.Date = pcal.Date AND pcal.PersonId = @PersonId
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	   AND @PersonId IS NOT NULL
	   AND @PracticeManagerId IS NULL
	UNION ALL
	SELECT cal.Date, ISNULL(pcal.DayOff, cal.DayOff) AS DayOff, @PersonId AS PersonId,
	       ISNULL(pcal.CompanyDayOff, cal.DayOff) AS CompanyDayOff,
	       CAST(CASE WHEN pcal.Date IS NULL OR pcal.Date < GETDATE() THEN 1 ELSE 0 END AS BIT) AS [ReadOnly]
	  FROM dbo.Calendar AS cal
	       LEFT JOIN dbo.v_PersonCalendar AS pcal ON cal.Date = pcal.Date AND pcal.PersonId = @PersonId
	       INNER JOIN dbo.Person AS p ON p.PersonId = @PersonId
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	   AND @PersonId IS NOT NULL /*AND @PracticeManagerId IS NOT NULL*/
	   AND p.DefaultPractice <> 4 /* Administration */
	   AND @PracticeManagerId IS NOT NULL
	ORDER BY cal.Date

