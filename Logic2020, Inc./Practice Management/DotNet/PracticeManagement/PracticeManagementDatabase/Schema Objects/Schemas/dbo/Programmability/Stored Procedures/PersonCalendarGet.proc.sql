-- =============================================
-- Author:		Sainath CH
-- Create date: 2012-06-01
-- =============================================
CREATE PROCEDURE [dbo].[PersonCalendarGet]
    (
      @StartDate DATETIME ,
      @EndDate DATETIME ,
      @PersonId INT ,
      @PracticeManagerId INT
    )
AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @HolidayTimeTypeId  INT,
			@ORTTimeTypeId		INT,
			@UnpaidTimeTypeId	INT,
			@Today				DATE

    SELECT  @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId() ,
            @ORTTimeTypeId = dbo.GetORTTimeTypeId(),
			@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId(),
			@Today	= CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE()))

	SELECT  CAL.Date ,
            ISNULL(PCAL.DayOff, CAL.DayOff) AS DayOff ,
            ISNULL(PCAL.CompanyDayOff, CAL.DayOff) AS CompanyDayOff ,
            CAST(CASE WHEN PCAL.Date IS NULL THEN 1 ELSE 0 END AS BIT) AS [ReadOnly],
            ( CASE WHEN CAL.DayOff = 1 THEN CAL.HolidayDescription
                   WHEN PCAL.DayOff = 1
                        AND PCAL.TimeTypeId IN ( @HolidayTimeTypeId,
                                                 @ORTTimeTypeId ,
												 @UnpaidTimeTypeId)
										THEN PCAL.Description + dbo.GetApprovedByName(CAL.Date,PCAL.TimeTypeId,@PersonId)
                   WHEN PCAL.DayOff = 1 THEN pcal.Description
                   ELSE ''
              END ) AS HolidayDescription ,
            CASE WHEN CAL.DayOff = 1 THEN NULL
				 WHEN PCAL.DayOff = 1 THEN PCAL.ActualHours
			END AS ActualHours,
            ISNULL(PCAL.IsFloatingHoliday,0) AS [IsFloatingHoliday] ,
            PCAL.TimeTypeId,
			CASE WHEN @UnpaidTimeTypeId = PCAL.TimeTypeId THEN 1 ELSE 0 END AS [IsUnpaidTimeType]
    FROM    dbo.Calendar AS CAL
            LEFT JOIN dbo.v_PersonCalendar AS PCAL ON CAL.Date = PCAL.Date
                                                      AND PCAL.PersonId = @PersonId
            LEFT JOIN dbo.Person AS AP ON AP.PersonId = PCAL.ApprovedBy
    WHERE   cal.Date BETWEEN @StartDate AND @EndDate
            AND @PersonId IS NOT NULL
            AND @PracticeManagerId IS NULL
    UNION ALL
    SELECT  CAL.Date ,
            ISNULL(PCAL.DayOff, CAL.DayOff) AS DayOff ,
            ISNULL(PCAL.CompanyDayOff, CAL.DayOff) AS CompanyDayOff ,
            CAST(CASE WHEN	PCAL.Date IS NULL 
							OR CONVERT(DATE, PCAL.Date) < @Today
							THEN 1 
						ELSE 0 
				END AS BIT) AS [ReadOnly],
            ( CASE WHEN CAL.DayOff = 1 THEN CAL.HolidayDescription
                   WHEN PCAL.DayOff = 1 AND PCAL.TimeTypeId IN ( @HolidayTimeTypeId,@ORTTimeTypeId,@UnpaidTimeTypeId)
										THEN PCAL.Description + dbo.GetApprovedByName(CAL.Date,PCAL.TimeTypeId,@PersonId)
                   WHEN PCAL.DayOff = 1 THEN PCAL.Description
                   ELSE ''
              END ) AS HolidayDescription ,
			CASE WHEN CAL.DayOff = 1 THEN NULL
				 WHEN PCAL.DayOff = 1 THEN PCAL.ActualHours
			END AS ActualHours,
            ISNULL(PCAL.IsFloatingHoliday,0) AS [IsFloatingHoliday] ,
            PCAL.TimeTypeId,
			CASE WHEN @UnpaidTimeTypeId = PCAL.TimeTypeId THEN 1 ELSE 0 END AS [IsUnpaidTimeType]
    FROM    dbo.Calendar AS CAL
            LEFT JOIN dbo.v_PersonCalendar AS PCAL ON CAL.Date = PCAL.Date
                                                      AND PCAL.PersonId = @PersonId
            INNER JOIN dbo.Person AS P ON P.PersonId = @PersonId
            LEFT JOIN dbo.Person AS AP ON AP.PersonId = PCAL.ApprovedBy
    WHERE   CAL.Date BETWEEN @StartDate AND @EndDate
            AND @PersonId IS NOT NULL 
			/*AND @PracticeManagerId IS NOT NULL*/
--      As per 2961 any person can view any persons calendar
--	   AND (p.DefaultPractice <> 4 /* Administration */
--			OR @PersonId = @PracticeManagerId)
            AND @PracticeManagerId IS NOT NULL
    ORDER BY CAL.Date
END

