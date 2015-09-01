CREATE PROCEDURE [dbo].[GetLocations]
AS
BEGIN

	SELECT L.LocationId,L.LocationCode,L.LocationName,L.ParentId,P.LocationCode AS ParentLocationCode, L.TimeZone,L.Country
	FROM dbo.Location L
	LEFT JOIN dbo.Location P ON P.LocationId = L.ParentId
	
END
