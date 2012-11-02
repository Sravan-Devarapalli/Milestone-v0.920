CREATE PROCEDURE [dbo].[GetSickLeaveTimeType]
AS
BEGIN
  SELECT  TT.TimeTypeId,
		  TT.Name
	FROM dbo.TimeType AS TT
    WHERE   TT.TimeTypeId = dbo.[GetSickLeaveTimeTypeId]()
END
	
