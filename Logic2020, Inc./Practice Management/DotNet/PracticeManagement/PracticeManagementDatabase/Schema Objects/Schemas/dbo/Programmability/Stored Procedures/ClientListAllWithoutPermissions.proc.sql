CREATE PROCEDURE [dbo].[ClientListAllWithoutPermissions]
AS
BEGIN
	SELECT ClientId,Name 
	FROM dbo.Client 
	WHERE IsInternal = 0
END

