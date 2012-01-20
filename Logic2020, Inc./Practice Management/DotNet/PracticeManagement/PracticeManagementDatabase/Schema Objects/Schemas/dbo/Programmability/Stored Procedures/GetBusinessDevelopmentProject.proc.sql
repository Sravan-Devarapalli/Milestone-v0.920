CREATE PROCEDURE [dbo].[GetBusinessDevelopmentProject]
AS
BEGIN
	SELECT  ProjectId,Name,ProjectNumber
	FROM [dbo].[Project]
	WHERE ProjectNumber = 'P999918'
END
