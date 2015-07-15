CREATE FUNCTION [dbo].[SmallerDateBetweenTwo]
(
	@Date1	DATETIME,
	@Date2	DATETIME
)
RETURNS DATETIME
AS
BEGIN
	
	 IF @Date1 < ISNULL(@Date2,@Date1)
        RETURN @Date1
    ELSE
        RETURN @Date2

    RETURN NULL

END
