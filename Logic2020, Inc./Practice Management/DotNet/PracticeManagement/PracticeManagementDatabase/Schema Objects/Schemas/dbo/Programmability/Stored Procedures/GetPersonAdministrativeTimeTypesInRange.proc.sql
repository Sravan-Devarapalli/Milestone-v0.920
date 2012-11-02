-- =============================================
-- Author:		Sainath CH
-- Create date: 2012-10-04
-- Description:	Gets administrative time types for the given person pay in the given range.
-- =============================================  
CREATE PROCEDURE dbo.[GetPersonAdministrativeTimeTypesInRange]
(
  @PersonId		  INT = NULL,
  @StartDate      DATETIME = NULL,
  @EndDate        DATETIME = NULL,
  @IncludePTO     BIT = 0,
  @IncludeHoliday BIT = 0,
  @IncludeUnpaid  BIT = 0,
  @IncludeSickLeave BIT = 0
)
AS
BEGIN

	DECLARE @HolidayTimeTypeId	INT,
			@PTOTimeTypeId		INT,
			@UnpaidTimeTypeId	INT,
			@ORTTimeTypeId		INT,
			@SickLeaveTimeTypeId INT,
			@IsPersonW2Salary	BIT = 0,
			@IsPersonW2Hourly   BIT = 0

	SELECT @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(),
		   @PTOTimeTypeId = dbo.GetPTOTimeTypeId(),
		   @UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId(),
		   @SickLeaveTimeTypeId = dbo.GetSickLeaveTimeTypeId(),
		   @ORTTimeTypeId = dbo.GetORTTimeTypeId()

	DECLARE @NotIncludedWorkTypes TABLE (ID INT)

	INSERT INTO @NotIncludedWorkTypes 
	SELECT CASE WHEN @IncludePTO = 0 THEN  @PTOTimeTypeId ELSE -1 END 
	UNION
	SELECT CASE WHEN @IncludeHoliday = 0 THEN  @HolidayTimeTypeId ELSE -1 END 
	UNION 
	SELECT CASE WHEN @IncludeUnpaid = 0 THEN  @UnpaidTimeTypeId ELSE -1 END 
	UNION 
	SELECT CASE WHEN @IncludeSickLeave = 0 THEN  @SickLeaveTimeTypeId ELSE -1 END 

	SELECT @IsPersonW2Salary = 1 FROM dbo.Pay p WHERE p.Person = @PersonId AND p.StartDate <= @EndDate AND p.EndDate-1 >= @StartDate AND p.Timescale = 2 --w2-salary
	SELECT @IsPersonW2Hourly = 1 FROM dbo.Pay p WHERE p.Person = @PersonId AND p.StartDate <= @EndDate AND p.EndDate-1 >= @StartDate AND p.Timescale = 1 --w2-hourly

	SELECT TT.TimeTypeId, 
		   TT.[Name],
		   CASE WHEN TT.TimeTypeId = @ORTTimeTypeId THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END [IsORTTimeType],
		   CASE WHEN TT.TimeTypeId = @UnpaidTimeTypeId THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END [IsUnpaidTimeType],
		   TT.IsW2HourlyAllowed,
		   TT.IsW2SalaryAllowed
	FROM dbo.TimeType AS TT
	--Joined ProjectTimeType because when new Administrative time type is added and not attached to any project
		--It should not be visible in the time entry and calendar page.
	INNER JOIN dbo.ProjectTimeType PTT ON TT.IsAdministrative = 1 AND TT.TimeTypeId = PTT.TimeTypeId AND PTT.IsAllowedToShow = 1 AND TT.IsActive = 1
	WHERE (
			(
				(@IsPersonW2Hourly = 0 OR (@IsPersonW2Hourly = 1 AND TT.IsW2HourlyAllowed = 1 ))
				AND
				(@IsPersonW2Salary = 0 OR (@IsPersonW2Salary = 1 AND TT.IsW2SalaryAllowed = 1 ))
			)
			OR
			(
				(@IsPersonW2Salary = 1 AND @IsPersonW2Hourly = 1) AND ( TT.IsW2SalaryAllowed = 1 OR TT.IsW2HourlyAllowed = 1)
			)
		  )
			AND TT.TimeTypeId NOT IN (SELECT ID FROM @NotIncludedWorkTypes)
	ORDER BY TT.[Name]
	
END

