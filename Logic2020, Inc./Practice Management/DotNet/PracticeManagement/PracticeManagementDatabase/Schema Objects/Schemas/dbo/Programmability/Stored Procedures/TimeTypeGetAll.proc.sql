-- =============================================
-- Author:		ThulasiRam.P
-- Modified date: 2012-02-14
-- Description:	Gets all avaliable time types
-- =============================================
CREATE PROCEDURE dbo.TimeTypeGetAll
AS
BEGIN

DECLARE @NOW DATETIME = dbo.gettingPmtime(GETUTCDATE())

	;WITH UsedTimeTypes AS
	(
		SELECT CC.TimeTypeId,MAX(TE.ChargeCodeDate) as MaxUsedDate 
		FROM dbo.TimeEntry TE
		INNER JOIN ChargeCode CC ON CC.Id = TE.ChargeCodeId
		GROUP BY CC.TimeTypeId
	)

	SELECT 
		tt.TimeTypeId, 
		tt.[Name], 
		CASE WHEN UTT.TimeTypeId IS NOT NULL THEN 'True'
				ELSE 'False' END AS InUse,
		CASE WHEN UTT.TimeTypeId IS NOT NULL AND UTT.MaxUsedDate > @NOW THEN 1
				ELSE 0 END AS InFutureUse,
		tt.IsDefault,
		tt.IsAllowedToEdit ,
		tt.IsActive,
		tt.IsInternal,
		tt.IsAdministrative
	FROM dbo.TimeType AS tt
	LEFT JOIN UsedTimeTypes UTT ON UTT.TimeTypeId = tt.TimeTypeId
	ORDER BY tt.Name
END

