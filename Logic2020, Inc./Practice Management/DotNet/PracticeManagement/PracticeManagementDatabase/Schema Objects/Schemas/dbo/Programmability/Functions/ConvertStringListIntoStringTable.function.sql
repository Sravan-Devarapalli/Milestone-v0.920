CREATE FUNCTION [dbo].[ConvertStringListIntoStringTable]
(
    @OrderList NVARCHAR(MAX)
)
RETURNS @ParsedList TABLE
(
    ResultName NVARCHAR(MAX) 
)
AS
BEGIN
    DECLARE @SplitLength INT, @Delimiter VARCHAR(5)
    SET @OrderList = LTRIM(RTRIM(@OrderList))
    SET @Delimiter = ','
    
    WHILE LEN(@OrderList) > 0
    BEGIN 
        SELECT @SplitLength = (CASE CHARINDEX(@Delimiter,@OrderList) WHEN 0 THEN
            LEN(@OrderList) ELSE CHARINDEX(@Delimiter,@OrderList) -1 END)
 
        INSERT INTO @ParsedList
        SELECT SUBSTRING(@OrderList,1,@SplitLength) 
    
        SELECT @OrderList = (CASE (len(@OrderList) - @SplitLength) WHEN 0 THEN  ''
            ELSE RIGHT(@OrderList, len(@OrderList) - @SplitLength - 1) END)
    END 
RETURN  
END
