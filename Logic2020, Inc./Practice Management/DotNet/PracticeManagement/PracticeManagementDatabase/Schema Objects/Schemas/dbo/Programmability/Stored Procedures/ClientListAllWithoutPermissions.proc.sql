CREATE PROCEDURE [dbo].[ClientListAllWithoutPermissions]
AS
BEGIN
	SELECT ClientId,Name 
	FROM dbo.Client C
	ORDER BY C.Name
END

