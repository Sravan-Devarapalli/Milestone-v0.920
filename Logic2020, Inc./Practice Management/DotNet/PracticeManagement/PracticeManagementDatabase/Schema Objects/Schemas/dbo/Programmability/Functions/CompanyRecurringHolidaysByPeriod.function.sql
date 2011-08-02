CREATE FUNCTION [dbo].[CompanyRecurringHolidaysByPeriod]
(	
	@StartDate DATETIME,
	@EndDate   DATETIME
)
RETURNS 
@HoliDayListWithDates table
(
	Date				DATETIME
	,DayOff				BIT
	,Id					INT
	,[Description]		NVARCHAR(255)
	,[IsSet]			BIT
	,[Month]			INT
	,[Day]				INT
	,[NumberInMonth]	INT
	,[DayOfTheWeek]		INT
	,[DateDescription]	NVARCHAR(255)
) 
AS
BEGIN 
	INSERT INTO @HoliDayListWithDates
	SELECT c1.Date
			,c1.DayOff
			,crh.Id
			,crh.[Description]
			,crh.[IsSet]
			,crh.[Month]
			,crh.[Day]
			,crh.[NumberInMonth]
			,crh.[DayOfTheWeek]
			,crh.[DateDescription]
	FROM Calendar c1
	JOIN CompanyRecurringHoliday crh ON 
			(
					(crh.[Day] IS NOT NULL --If holiday is on exact Date.
						AND (	--If Holiday comes in 
								DAY(c1.[Date]) = crh.[Day] AND MONTH(c1.[Date]) = crh.[Month] AND DATEPART(DW,c1.[Date]) NOT IN(1,7)
								OR DAY(DATEADD(DD,1,c1.[Date])) = crh.[Day] AND MONTH(DATEADD(DD,1,c1.[Date])) = crh.[Month]  AND DATEPART(DW,c1.[Date]) = 6
								OR DAY(DATEADD(DD,-1,c1.[Date])) = crh.[Day] AND MONTH(DATEADD(DD,-1,c1.[Date])) = crh.[Month] AND DATEPART(DW,c1.[Date]) = 2
							)
					 )
					 OR
					 ( crh.[Day] IS NULL AND MONTH(c1.[Date]) = crh.[Month]
						AND (
								DATEPART(DW,c1.[Date]) = crh.DayOfTheWeek
								AND (
										(crh.NumberInMonth IS NOT NULL
										 AND  (c1.[Date] - DAY(c1.[Date])+1) -
														CASE WHEN (DATEPART(DW,c1.[Date]-DAY(c1.[Date])+1))%7 <= crh.DayOfTheWeek 
															 THEN (DATEPART(DW,c1.[Date]-DAY(c1.[Date])+1))%7
															 ELSE (DATEPART(DW,c1.[Date]-DAY(c1.[Date])+1)-7)
															 END +(7*(crh.NumberInMonth-1))+crh.DayOfTheWeek = [Date]
										 )
										 OR( crh.NumberInMonth IS NULL 
											 AND (DATEADD(MM,1,c1.[Date] - DAY(c1.[Date])+1)- 1) - 
													 (CASE WHEN DATEPART(DW,(DATEADD(MM,1,c1.[Date] - DAY(c1.[Date])+1)- 1)) >= crh.DayOfTheWeek
														   THEN (DATEPART(DW,(DATEADD(MM,1,c1.[Date] - DAY(c1.[Date])+1)- 1)))-7
														   ELSE (DATEPART(DW,(DATEADD(MM,1,c1.[Date] - DAY(c1.[Date])+1)- 1)))
														   END)-(7-crh.DayOfTheWeek)= c1.[Date]
										   )
									 )
							)
						
					 )
				 
				)	
	WHERE Date BETWEEN @StartDate AND ISNULL(@EndDate, dbo.GetFutureDate())

	RETURN
END
