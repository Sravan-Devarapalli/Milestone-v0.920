CREATE PROCEDURE [dbo].[GetInternalBusinessUnits]
AS
BEGIN

	SELECT [GroupId] ,[Code],[Name]  
		FROM dbo.ProjectGroup 
		WHERE ClientId = (SELECT [ClientId] 
								FROM [dbo].[Client] 
								WHERE Code = 'C2020')

END

