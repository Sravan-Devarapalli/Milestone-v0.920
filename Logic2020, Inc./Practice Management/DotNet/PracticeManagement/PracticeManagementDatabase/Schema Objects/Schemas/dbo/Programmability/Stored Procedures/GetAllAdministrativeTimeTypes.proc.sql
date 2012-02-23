-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 2012-02-10
-- Description:	Gets all administrative time types
-- =============================================
CREATE PROCEDURE dbo.GetAllAdministrativeTimeTypes
(
  @IncludePTO BIT = 0,
  @IncludeHoliday BIT = 0
)
AS
BEGIN

DECLARE @HolidayTimeTypeId	INT,
		@PTOTimeTypeId		INT

SELECT @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(),
	   @PTOTimeTypeId = dbo.GetPTOTimeTypeId()



	SELECT tt.TimeTypeId, 
		   tt.[Name]
	FROM dbo.TimeType AS tt
		INNER JOIN dbo.ProjectTimeType ptt ON tt.IsAdministrative = 1 AND tt.TimeTypeId = ptt.TimeTypeId AND ptt.IsAllowedToShow = 1 AND tt.IsActive = 1
	WHERE
	(@IncludePTO = 0 AND @IncludeHoliday = 0 AND tt.TimeTypeId NOT IN (@PTOTimeTypeId,@HolidayTimeTypeId)) OR
	(@IncludePTO = 0 AND @IncludeHoliday = 1 AND tt.TimeTypeId NOT IN (@PTOTimeTypeId)) OR
	(@IncludePTO = 1 AND @IncludeHoliday = 0 AND tt.TimeTypeId NOT IN (@HolidayTimeTypeId)) OR
	(@IncludePTO = 1 AND @IncludeHoliday = 1 )
	ORDER BY tt.[Name]
	
END

