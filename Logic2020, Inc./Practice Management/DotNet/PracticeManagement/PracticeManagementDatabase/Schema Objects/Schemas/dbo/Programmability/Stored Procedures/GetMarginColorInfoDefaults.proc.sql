CREATE PROCEDURE [dbo].[GetMarginColorInfoDefaults]
(
	@GoalTypeId INT
)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT cmci.[ColorId]
		  ,cmci.[StartRange]
		  ,cmci.[EndRange]
		  ,color.Value
		  ,color.Description
	  FROM [dbo].DefaultMarginColorInfo AS cmci 
	  INNER JOIN dbo.Color color ON cmci.[ColorId] = color.Id
	  WHERE cmci.GoalTypeId =@GoalTypeId
	  ORDER BY cmci.StartRange
END
GO
