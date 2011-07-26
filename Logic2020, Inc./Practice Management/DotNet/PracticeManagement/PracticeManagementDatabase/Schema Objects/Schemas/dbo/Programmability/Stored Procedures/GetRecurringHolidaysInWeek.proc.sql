CREATE PROCEDURE [dbo].[GetRecurringHolidaysInWeek]
(
	@PersonId	INT,
	@StartDate  DateTime
)
AS
BEGIN
	
	SELECT C.[Date],
		   CRH.[Description]
	FROM dbo.Calendar C
	JOIN dbo.Pay P 
	ON C.[Date] BETWEEN P.StartDate AND P.EndDate-1
		AND	P.Person = @PersonId AND P.Timescale = 2 --W2-Salary
	JOIN dbo.CompanyRecurringHoliday CRH
	ON ((MONTH(C.[Date]) = CRH.[Month]
			AND (
					(CRH.[Day] IS NOT NULL --If holiday is on exact Date.
						AND (	--If Holiday comes in WeekEnds then giving prior to nearest weekday.
								DAY(C.[Date]) = CRH.[Day] AND DATEPART(DW,C.[Date]) NOT IN(1,7)
								OR DAY(DATEADD(DD,1,C.[Date])) = CRH.[Day] AND DATEPART(DW,C.[Date]) = 6
								OR DAY(DATEADD(DD,-1,C.[Date])) = CRH.[Day] AND DATEPART(DW,C.[Date]) = 2
							)
					 )
					 OR
					 ( CRH.[Day] IS NULL
						AND (
								DATEPART(DW,C.[Date]) = CRH.[DayOfTheWeek]
								AND (
										(CRH.NumberInMonth IS NOT NULL
										 AND  (C.[Date] - DAY(C.[Date])+1) -
														CASE WHEN (DATEPART(DW,C.[Date]-DAY(C.[Date])+1))%7 <= CRH.[DayOfTheWeek] 
															 THEN (DATEPART(DW,C.[Date]-DAY(C.[Date])+1))%7
															 ELSE (DATEPART(DW,C.[Date]-DAY(C.[Date])+1)-7)
															 END +(7*(CRH.NumberInMonth-1))+CRH.[DayOfTheWeek] = C.[Date]
										 )
										 OR( CRH.NumberInMonth IS NULL --For the last week holidays.
											 AND (DATEADD(MM,1,C.[Date] - DAY(C.[Date])+1)- 1) - 
													 (CASE WHEN DATEPART(DW,(DATEADD(MM,1,C.[Date] - DAY(C.[Date])+1)- 1)) >= CRH.[DayOfTheWeek]
														   THEN (DATEPART(DW,(DATEADD(MM,1,C.[Date] - DAY(C.[Date])+1)- 1)))-7
														   ELSE (DATEPART(DW,(DATEADD(MM,1,C.[Date] - DAY(C.[Date])+1)- 1)))
														   END)-(7-CRH.[DayOfTheWeek])= C.[Date]
										   )
									 )
							)
						
					 )
				 
				)
				)
				OR 
				(
					MONTH(C.[Date])%12 = CRH.Month-1
					AND MONTH(DATEADD(DD,1,C.[Date])) = CRH.Month
					AND CRH.[Day] IS NOT NULL
					AND DAY(DATEADD(DD,1,C.[Date])) = CRH.[Day] AND DATEPART(DW,C.[Date]) = 6
				))
	WHERE C.DayOff = 1
		AND C.Date Between DATEADD(DD,1-DATEPART(DW,@StartDate),@StartDate)
			        AND DATEADD(DD,7-DATEPART(DW,@StartDate),@StartDate)
		
	
END

GO
