CREATE PROCEDURE [dbo].[GetInternalBusinessUnits]
AS
BEGIN

	SELECT [GroupId] ,[Code],[Name]  
	FROM dbo.ProjectGroup 
	WHERE IsInternal = 1
END
