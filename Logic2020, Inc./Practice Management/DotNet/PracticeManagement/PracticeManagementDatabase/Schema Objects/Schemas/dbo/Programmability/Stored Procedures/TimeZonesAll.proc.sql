CREATE PROCEDURE [dbo].[TimeZonesAll]
AS
	SELECT id,
			GMT,
			GMTName,
			IsActive
	FROM TimeZones
