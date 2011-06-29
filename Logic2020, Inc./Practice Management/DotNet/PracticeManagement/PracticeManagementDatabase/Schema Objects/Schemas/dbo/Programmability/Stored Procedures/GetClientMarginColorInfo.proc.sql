CREATE PROCEDURE [dbo].[GetClientMarginColorInfo]
(
	@ClientId INT
)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT cmci.[ColorId]
		  ,cmci.[StartRange]
		  ,cmci.[EndRange]
		  ,color.Value
		  ,color.Description
	  FROM [dbo].[ClientMarginColorInfo] AS cmci 
	  INNER JOIN dbo.Color color ON cmci.[ColorId] = color.Id
	  WHERE cmci.ClientId =@ClientId
	  ORDER BY cmci.StartRange
END
GO
