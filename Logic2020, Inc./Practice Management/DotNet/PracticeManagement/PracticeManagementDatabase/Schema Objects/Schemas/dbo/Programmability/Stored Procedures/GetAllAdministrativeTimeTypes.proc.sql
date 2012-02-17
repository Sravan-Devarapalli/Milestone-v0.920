-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 2012-02-10
-- Description:	Gets all administrative time types
-- =============================================
CREATE PROCEDURE dbo.GetAllAdministrativeTimeTypes
(
  @IncludePTOAndHoliday BIT = 0
)
AS
BEGIN

DECLARE @HolidayTimeTypeId	INT,
		@PTOTimeTypeId		INT

SELECT @HolidayTimeTypeId = dbo.GetPTOTimeTypeId(),
	   @PTOTimeTypeId = dbo.GetHolidayTimeTypeId()



	SELECT tt.TimeTypeId, 
		   tt.[Name]
	FROM dbo.TimeType AS tt
		INNER JOIN dbo.ProjectTimeType ptt ON tt.IsAdministrative = 1 AND tt.TimeTypeId = ptt.TimeTypeId AND ptt.IsAllowedToShow = 1 AND tt.IsActive = 1
	WHERE
	((@IncludePTOAndHoliday = 0 AND tt.TimeTypeId NOT IN (@HolidayTimeTypeId,@PTOTimeTypeId)) OR (@IncludePTOAndHoliday = 1))
	
END

