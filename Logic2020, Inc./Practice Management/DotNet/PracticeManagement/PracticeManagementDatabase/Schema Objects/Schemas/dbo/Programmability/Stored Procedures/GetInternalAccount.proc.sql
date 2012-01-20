CREATE PROCEDURE [dbo].[GetInternalAccount]
AS
BEGIN
	SELECT [ClientId],[Code],[Name] 
	FROM [dbo].[Client] 
	WHERE Code = 'C2020'
END
