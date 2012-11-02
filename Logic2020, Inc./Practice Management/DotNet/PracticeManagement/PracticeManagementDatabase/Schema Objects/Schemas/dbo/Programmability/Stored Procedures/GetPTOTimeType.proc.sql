CREATE PROCEDURE [dbo].[GetPTOTimeType]
AS
BEGIN
  SELECT  TT.TimeTypeId,
		  TT.Name
	FROM dbo.TimeType AS TT
    WHERE   TT.TimeTypeId = dbo.[GetPTOTimeTypeId]()
END
	
