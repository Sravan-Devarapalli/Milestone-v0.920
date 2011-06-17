CREATE FUNCTION [dbo].[ConvertStringListIntoTableWithTwoColoumns]
(
	@OrderList varchar(max)
)
RETURNS 
@ParsedList table
(
	ResultId int,
	ResultType int
)
AS
BEGIN
	DECLARE @ResultId nvarchar(256),@ResultType nvarchar(256), @Pos int

	SET @OrderList = LTRIM(RTRIM(@OrderList))+ ','
	SET @Pos = CHARINDEX(',', @OrderList, 1)

	IF REPLACE(@OrderList, ',', '') <> ''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @ResultId = LTRIM(RTRIM(LEFT(@OrderList, @Pos - 1)))
			IF @ResultId <> ''
			BEGIN
				SET	@ResultType =SUBSTRING(@ResultId,CHARINDEX(':',@ResultId)+1,len(@ResultId))
				SET @ResultId = SUBSTRING(@ResultId,0,CHARINDEX(':',@ResultId))
				
				INSERT INTO @ParsedList (ResultId , ResultType) 
				VALUES (CAST(@ResultId AS int),CAST(@ResultType AS int))
			END
			SET @OrderList = RIGHT(@OrderList, LEN(@OrderList) - @Pos)
			SET @Pos = CHARINDEX(',', @OrderList, 1)

		END
	END	
	RETURN
END


