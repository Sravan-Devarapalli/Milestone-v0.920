-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 2012-02-10
-- Updated by:	Sainath CH
-- Update date: 03-27-2012
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



	SELECT TT.TimeTypeId, 
		   TT.[Name],
		   CASE TT.[Name] WHEN 'Other Reportable Time' THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END 'IsORTTimeType'
	FROM dbo.TimeType AS TT
	WHERE TT.IsAdministrative = 1 AND
	(
	(@IncludePTO = 0 AND @IncludeHoliday = 0 AND TT.TimeTypeId NOT IN (@PTOTimeTypeId,@HolidayTimeTypeId)) OR
	(@IncludePTO = 0 AND @IncludeHoliday = 1 AND TT.TimeTypeId NOT IN (@PTOTimeTypeId)) OR
	(@IncludePTO = 1 AND @IncludeHoliday = 0 AND TT.TimeTypeId NOT IN (@HolidayTimeTypeId)) OR
	(@IncludePTO = 1 AND @IncludeHoliday = 1 )
	)
	ORDER BY TT.[Name]
	
END

