-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Description:  Gets the WorkType List InUse Details.
-- =========================================================================
CREATE PROCEDURE [dbo].[GetTimeTypesInUseDetailsByProject]
    (
      @ProjectId INT ,
      @TimeTypeIds NVARCHAR(MAX)
    )
AS 
    BEGIN

        SELECT  TT.TimeTypeId ,
                TT.Name ,
                MAX(CASE WHEN TE.ChargeCodeId IS NULL THEN 0
                         ELSE 1
                    END) AS [InUse]
        FROM    dbo.TimeType TT
                LEFT JOIN dbo.ChargeCode CC ON TT.TimeTypeId = CC.TimeTypeId
                                               AND CC.ProjectId = @ProjectId
                LEFT JOIN dbo.TimeEntry TE ON CC.ID = TE.ChargeCodeId
        WHERE   TT.TimeTypeId IN (
                SELECT  ResultId
                FROM    dbo.ConvertStringListIntoTable(@TimeTypeIds) )
        GROUP BY TT.TimeTypeId ,
                TT.Name

    END
