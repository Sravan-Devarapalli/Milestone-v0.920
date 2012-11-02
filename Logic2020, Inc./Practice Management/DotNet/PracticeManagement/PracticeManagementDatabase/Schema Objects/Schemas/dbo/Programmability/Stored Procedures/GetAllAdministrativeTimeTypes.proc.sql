-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 2012-02-10
-- Updated by:	Sainath CH
-- Update date: 06-01-2012
-- Description:	Gets all administrative time types
-- =============================================  
CREATE PROCEDURE dbo.GetAllAdministrativeTimeTypes
(
  @IncludePTO BIT = 0,
  @IncludeHoliday BIT = 0,
  @IncludeUnpaid BIT = 0,
  @IncludeSickLeave BIT = 0
)
AS
BEGIN

DECLARE @HolidayTimeTypeId	INT,
		@PTOTimeTypeId		INT,
		@UnpaidTimeTypeId	INT,
		@ORTTimeTypeId		INT,
		@SickLeaveTimeTypeId		INT

SELECT @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(),
	   @PTOTimeTypeId = dbo.GetPTOTimeTypeId(),
	   @UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId(),
	   @ORTTimeTypeId = dbo.GetORTTimeTypeId(),
	   @SickLeaveTimeTypeId = dbo.[GetSickLeaveTimeTypeId]()

	   DECLARE @NotIncludedWorkTypes TABLE (ID INT)
	   INSERT INTO @NotIncludedWorkTypes 
	   SELECT CASE WHEN @IncludePTO = 0 THEN  @PTOTimeTypeId ELSE -1 END 
	   UNION
	   SELECT CASE WHEN @IncludeHoliday = 0 THEN  @HolidayTimeTypeId ELSE -1 END 
	   UNION 
	   SELECT CASE WHEN @IncludeUnpaid = 0 THEN  @UnpaidTimeTypeId ELSE -1 END 
	   UNION 
	   SELECT CASE WHEN @IncludeSickLeave = 0 THEN  @SickLeaveTimeTypeId ELSE -1 END 


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
	WHERE 
	TT.TimeTypeId NOT IN (SELECT ID FROM @NotIncludedWorkTypes)
	ORDER BY TT.[Name]
	
END

