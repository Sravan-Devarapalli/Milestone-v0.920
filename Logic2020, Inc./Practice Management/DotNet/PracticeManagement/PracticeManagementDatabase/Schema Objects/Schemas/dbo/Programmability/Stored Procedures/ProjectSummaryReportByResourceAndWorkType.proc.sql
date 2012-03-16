-- =========================================================================
-- Author:		ThulasiRam.P
-- Create date: 03-15-2012
-- Description:  Time Entries grouped by workType and Resource for a Project.
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByResourceAndWorkType]
(
	@ProjectNumber NVARCHAR(12),
	@PersonRoleIds NVARCHAR(MAX) = NULL,
	@OrderByCerteria NVARCHAR(20) = 'resource'-- resource,person role,total
)
AS
BEGIN

	DECLARE @ProjectId INT 

	SELECT @ProjectId = P.ProjectId
	FROM dbo.Project AS P
	WHERE P.ProjectNumber = @ProjectNumber 


	SELECT 
	TE.PersonId,
	P.LastName,
	P.FirstName,
	CC.TimeTypeId,
	TT.Name TimeTypeName,
	ROUND(SUM(CASE WHEN TEH.IsChargeable = 1 THEN TEH.ActualHours ELSE 0 END),2) AS BillableHours,
	ROUND(SUM(CASE WHEN TEH.IsChargeable = 0 THEN TEH.ActualHours ELSE 0 END),2) AS NonBillableHours
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId 
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId 
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
	INNER JOIN dbo.TimeType AS TT ON TT.TimeTypeId = CC.TimeTypeId
	GROUP BY    TE.PersonId,
				CC.TimeTypeId,
				TT.Name,
				P.LastName,
				P.FirstName
	

END
