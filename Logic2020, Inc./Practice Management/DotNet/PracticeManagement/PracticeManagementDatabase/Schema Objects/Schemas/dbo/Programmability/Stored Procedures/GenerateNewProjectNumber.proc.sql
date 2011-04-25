CREATE PROCEDURE dbo.GenerateNewProjectNumber (@ProjectNumber AS NVARCHAR (12) OUTPUT) AS
BEGIN
DECLARE @StringCounter NVARCHAR(7)
	DECLARE @Counter INT

	SET @Counter = 0

	WHILE  (1 = 1)
	BEGIN

		SET @StringCounter = CAST(@Counter AS NVARCHAR(7))
		IF LEN ( @StringCounter ) = 1
			SET @StringCounter =  '0' + @StringCounter

		SET @ProjectNumber = dbo.MakeNumberFromDate('P', GETDATE()) + @StringCounter
	
		IF NOT EXISTS (SELECT 1 FROM [dbo].[Project] as p WHERE p.[ProjectNumber] = @ProjectNumber)
			BREAK

		SET @Counter = @Counter + 1
	END
END


