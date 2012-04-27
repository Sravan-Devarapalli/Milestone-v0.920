-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 04-27-2012
--@OrderList = 
--<names>
--		<name> value1 </name>
--		<name> value2 </name>
--		<name> value3 </name>
--	</names>
-- =========================================================================
CREATE FUNCTION [dbo].[ConvertXmlStringInToStringTable] ( @OrderList XML )
RETURNS @ParsedList TABLE
    (
      ResultString NVARCHAR(1024)
    )
AS 
    BEGIN
        SET ANSI_NULLS ON;
        IF ( @OrderList IS NOT NULL ) 
            BEGIN
                INSERT  INTO @ParsedList
                        ( ResultString
                        )
                        SELECT  t.c.value('.[1]', 'NVARCHAR(1024)') AS ResultString
                        FROM    @OrderList.nodes('Names/Name') AS t ( c )
            END
        RETURN
    END

