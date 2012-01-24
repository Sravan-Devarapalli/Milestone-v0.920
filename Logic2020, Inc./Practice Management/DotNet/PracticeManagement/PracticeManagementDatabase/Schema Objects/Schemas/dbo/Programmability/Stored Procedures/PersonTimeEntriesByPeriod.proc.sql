CREATE PROCEDURE [dbo].[PersonTimeEntriesByPeriod]
	@PersonId	INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
AS
BEGIN
	--List Of time entries with detail
	SELECT TE.TimeEntryId,
			TE.ChargeCodeId,
			TE.ChargeCodeDate,
			TEH.CreateDate,
			TEH.ModifiedDate,
			TEH.ActualHours,
			TE.ForecastedHours,
			TE.Note,
			TEH.IsChargeable,
			TEH.ReviewStatusId,
			TE.IsCorrect,
			CC.TimeTypeId,
			TEH.ModifiedBy,
			CASE WHEN CCH.ChargeCodeId IS NULL THEN CONVERT(BIT,0) ELSE CONVERT(BIT,1) END AS 'IsChargeCodeOff'
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours AS TEH  ON TE.TimeEntryId = TEH.TimeEntryId
	INNER JOIN ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
	LEFT JOIN dbo.ChargeCodeTurnOffHistory CCH ON CC.Id = CCH.ChargeCodeId AND TE.ChargeCodeDate BETWEEN CCH.StartDate AND CCH.EndDate
	
	--List of Charge codes with recursive flag.
	SELECT DISTINCT ISNULL(CC.TimeEntrySectionId, PTRS.TimeEntrySectionId) AS 'TimeEntrySectionId',
		CC.Id AS 'ChargeCodeId', 
		ISNULL(CC.ClientId, PTRS.ClientId) AS 'ClientId', 
		C.Name 'ClientName',
		ISNULL(CC.ProjectGroupId, PTRS.ProjectGroupId) AS 'GroupId', 
		PG.Name 'GroupName',
		ISNULL(CC.ProjectId, PTRS.ProjectId) AS 'ProjectId',
		p.ProjectNumber, 
		P.Name 'ProjectName',
		CASE WHEN PTRS.Id IS NOT NULL AND PTRS.EndDate IS NULL THEN 1 ELSE 0 END AS 'IsRecursive'
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
	FULL JOIN dbo.PersonTimeEntryRecursiveSelection PTRS 
		ON PTRS.ClientId = CC.ClientId AND ISNULL(PTRS.ProjectGroupId, 0) = ISNULL(CC.ProjectGroupId, 0) AND PTRS.ProjectId = CC.ProjectId AND StartDate < @EndDate AND EndDate > @StartDate 
	INNER JOIN Client C ON C.ClientId = ISNULL(CC.ClientId, PTRS.ClientId)
	LEFT JOIN ProjectGroup PG ON PG.GroupId = ISNULL(CC.ProjectGroupId, PTRS.ProjectGroupId)
	INNER JOIN Project P ON P.ProjectId = ISNULL(CC.ProjectId, PTRS.ProjectId)
	WHERE ISNULL(PTRS.PersonId, @PersonId) = @PersonId AND (CC.Id IS NULL AND PTRS.StartDate < @EndDate AND ISNULL(PTRS.EndDate,dbo.GetFutureDate()) > @StartDate) OR CC.Id IS NOT NULL
	UNION
	SELECT CC.TimeEntrySectionId AS 'TimeEntrySectionId',
		CC.Id AS 'ChargeCodeId', 
		CC.ClientId AS 'ClientId', 
		C.Name 'ClientName',
		CC.ProjectGroupId AS 'GroupId', 
		PG.Name 'GroupName',
		CC.ProjectId AS 'ProjectId', 
		p.ProjectNumber,
		P.Name 'ProjectName',
		0 AS 'IsRecursive'
	FROM ChargeCode CC
	INNER JOIN Client C ON C.ClientId = CC.ClientId AND CC.TimeEntrySectionId = 4--Administrative Section
	INNER JOIN Project P ON P.ProjectId = CC.ProjectId
	INNER JOIN ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
END

