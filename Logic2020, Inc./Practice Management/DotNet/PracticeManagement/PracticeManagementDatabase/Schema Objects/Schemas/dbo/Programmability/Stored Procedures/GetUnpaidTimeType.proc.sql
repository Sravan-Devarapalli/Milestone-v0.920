-- =============================================
-- Author:	Sainath C
-- Create date: 01-06-2012
-- =============================================
CREATE PROCEDURE [dbo].[GetUnpaidTimeType]
AS
BEGIN
  SELECT  TT.TimeTypeId,
		  TT.Name
	FROM dbo.TimeType AS TT
    WHERE   TT.TimeTypeId = dbo.GetUnpaidTimeTypeId()
END
	
