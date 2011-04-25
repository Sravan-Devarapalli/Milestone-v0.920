CREATE FUNCTION [dbo].[ConvertStringListIntoTable]
(
	@OrderList varchar(max)
)
RETURNS 
@ParsedList table
(
	ResultId int
)
AS
BEGIN
	DECLARE @ResultId varchar(10), @Pos int

	SET @OrderList = LTRIM(RTRIM(@OrderList))+ ','
	SET @Pos = CHARINDEX(',', @OrderList, 1)

	IF REPLACE(@OrderList, ',', '') <> ''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @ResultId = LTRIM(RTRIM(LEFT(@OrderList, @Pos - 1)))
			IF @ResultId <> ''
			BEGIN
				INSERT INTO @ParsedList (ResultId) 
				VALUES (CAST(@ResultId AS int)) --Use Appropriate conversion
			END
			SET @OrderList = RIGHT(@OrderList, LEN(@OrderList) - @Pos)
			SET @Pos = CHARINDEX(',', @OrderList, 1)

		END
	END	
	RETURN
END

