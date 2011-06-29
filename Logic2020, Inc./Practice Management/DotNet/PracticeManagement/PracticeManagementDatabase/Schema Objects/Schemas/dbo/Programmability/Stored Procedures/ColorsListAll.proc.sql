CREATE PROCEDURE dbo.ColorsListAll
AS
BEGIN
SET NOCOUNT ON;
	SELECT color.Id,color.Value,color.Description
	FROM dbo.Color as color
END

