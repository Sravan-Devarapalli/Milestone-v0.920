CREATE PROCEDURE [dbo].[SetRecurringHoliday]
(
	@Id INT,
	@IsSet	BIT
)
AS
BEGIN

DECLARE @Month int,
	@Day int,
	@NumberInMonth int,
	@DayOfTheWeek int,
	@Today DATETIME

SELECT @Today = dbo.GettingPMTime(GETUTCDATE())
SELECT
		@Month = [Month],
		@Day = [Day],
		@NumberInMonth= [NumberInMonth],
		@DayOfTheWeek = [DayOfTheWeek]
FROM dbo.CompanyRecurringHoliday
WHERE Id = @Id

	BEGIN TRY
	
	BEGIN TRANSACTION Tran_SetRecurringHoliday

	UPDATE dbo.CompanyRecurringHoliday
	SET IsSet = @IsSet
	WHERE Id = @Id

	UPDATE  C1
	SET DayOff = @IsSet
	FROM dbo.Calendar AS C1
	WHERE [Date] >= @Today
			AND ((MONTH([Date]) = @Month
			AND (
					(@Day IS NOT NULL --If holiday is on exact Date.
						AND (	--If Holiday comes in 
								DAY([Date]) = @Day AND DATEPART(DW,[Date]) NOT IN(1,7)
								OR DAY(DATEADD(DD,1,[Date])) = @Day AND DATEPART(DW,[Date]) = 6
								OR DAY(DATEADD(DD,-1,[Date])) = @Day AND DATEPART(DW,[Date]) = 2
							)
					 )
					 OR
					 ( @Day IS NULL
						AND (
								DATEPART(DW,[Date]) = @DayOfTheWeek
								AND (
										(@NumberInMonth IS NOT NULL
										 AND  ([Date] - DAY([Date])+1) -
														CASE WHEN (DATEPART(DW,[Date]-DAY([Date])+1))%7 <= @DayOfTheWeek 
															 THEN (DATEPART(DW,[Date]-DAY([Date])+1))%7
															 ELSE (DATEPART(DW,[Date]-DAY([Date])+1)-7)
															 END +(7*(@NumberInMonth-1))+@DayOfTheWeek = [Date]
										 )
										 OR( @NumberInMonth IS NULL 
											 AND (DATEADD(MM,1,[Date] - DAY([Date])+1)- 1) - 
													 (CASE WHEN DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)) >= @DayOfTheWeek
														   THEN (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))-7
														   ELSE (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))
														   END)-(7-@DayOfTheWeek)= [Date]
										   )
									 )
							)
						
					 )
				 
				)
				)
				OR 
				(
					MONTH([Date])%12 = @Month-1
					AND MONTH(DATEADD(DD,1,[Date])) = @Month
					AND @Day IS NOT NULL
					AND DAY(DATEADD(DD,1,[Date])) = @Day AND DATEPART(DW,[Date]) = 6
				)
				)

	
	UPDATE C1
	SET DayOff = @IsSet
	FROM dbo.PersonCalendarAuto C1
	WHERE [Date] >= @Today
			AND ((MONTH([Date]) = @Month
			AND (
					(@Day IS NOT NULL
						AND (
								DAY([Date]) = @Day AND DATEPART(DW,[Date]) NOT IN(1,7)
								OR DAY(DATEADD(DD,1,[Date])) = @Day AND DATEPART(DW,[Date]) = 6
								OR DAY(DATEADD(DD,-1,[Date])) = @Day AND DATEPART(DW,[Date]) = 2
							)
					 )
					 OR
					 ( @Day IS NULL
						AND (
								DATEPART(DW,[Date]) = @DayOfTheWeek
								AND (
										(@NumberInMonth IS NOT NULL
										 AND  ([Date] - DAY([Date])+1) -
														CASE WHEN (DATEPART(DW,[Date]-DAY([Date])+1))%7 <= @DayOfTheWeek 
															 THEN (DATEPART(DW,[Date]-DAY([Date])+1))%7
															 ELSE (DATEPART(DW,[Date]-DAY([Date])+1)-7)
															 END +(7*(@NumberInMonth-1))+@DayOfTheWeek = [Date]
										 )
										 OR( @NumberInMonth IS NULL 
											 AND (DATEADD(MM,1,[Date] - DAY([Date])+1)- 1) - 
													 (CASE WHEN DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)) >= @DayOfTheWeek
														   THEN (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))-7
														   ELSE (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))
														   END)-(7-@DayOfTheWeek)= [Date]
										   )
									 )
							)
						
					 )
				 
				))
				OR
				(
					MONTH([Date])%12 = @Month-1
					AND MONTH(DATEADD(DD,1,[Date])) = @Month
					AND @Day IS NOT NULL
					AND DAY(DATEADD(DD,1,[Date])) = @Day AND DATEPART(DW,[Date]) = 6
				)
				)
		
		COMMIT TRANSACTION Tran_SetRecurringHoliday	
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION Tran_SetRecurringHoliday
	END CATCH
					 
				
END
GO
