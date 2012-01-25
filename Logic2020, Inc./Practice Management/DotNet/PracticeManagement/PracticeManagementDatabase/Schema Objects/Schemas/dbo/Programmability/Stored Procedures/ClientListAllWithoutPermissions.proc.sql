CREATE PROCEDURE [dbo].[ClientListAllWithoutPermissions]
AS
BEGIN
	SELECT ClientId,Name 
	FROM dbo.Client 
END

