CREATE PROCEDURE [dbo].[AccountSummaryByBusinessDevelopment]
(
	@AccountIds	NVARCHAR(MAX),
	@BusinessUnitIds	NVARCHAR(MAX) = NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN
	
	DECLARE @StartDateLocal DATETIME ,
		@EndDateLocal DATETIME,
		@BusinessUnitIdsLocal	NVARCHAR(MAX),
		@AccountIdsLocal NVARCHAR(MAX),
		@FutureDate DATETIME

    DECLARE @AccountIdsTable TABLE ( Ids INT )

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)
		 , @BusinessUnitIdsLocal = @BusinessUnitIds
		 , @AccountIdsLocal = @AccountIds
		 , @FutureDate = dbo.GetFutureDate()

	INSERT INTO @AccountIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@AccountIdsLocal)

	
	SELECT PG.GroupId AS [BusinessUnitId]
		 , PG.Name AS [BusinessUnitName]
		 ,C.ClientId
		 ,C.Name AS ClientName
		 ,C.Code AS ClientCode
		 , PG.Code AS [GroupCode]
		 , PG.Active
		 , TT.Name AS [TimeTypeName]
		 , TT.Code AS [TimeTypeCode]
		 , P.PersonId
		 , P.EmployeeNumber
		 , P.FirstName
		 , P.LastName
		 , TE.ChargeCodeDate
		 , TEH.ActualHours AS NonBillableHours
		 , TE.Note
	FROM
		dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH
			ON TEH.TimeEntryId = TE.TimeEntryId AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
		INNER JOIN dbo.ChargeCode CC
			ON CC.Id = TE.ChargeCodeId AND CC.TimeEntrySectionId = 2 --Here 2 is Business Development section.
		INNER JOIN dbo.ProjectGroup PG
			ON PG.GroupId = CC.ProjectGroupId
		INNER JOIN dbo.TimeType AS TT
			ON TT.TimeTypeId = CC.TimeTypeId
		INNER JOIN dbo.Person P
			ON P.PersonId = TE.PersonId AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate, @FutureDate)
		INNER JOIN dbo.Client C ON C.ClientId = PG.ClientId

	WHERE
		CC.ClientId IN (SELECT Ids FROM @AccountIdsTable)
		AND (@BusinessUnitIdsLocal IS NULL
		OR PG.GroupId IN (SELECT ResultId
						  FROM
							  dbo.ConvertStringListIntoTable(@BusinessUnitIdsLocal)))
	ORDER BY
		P.LastName
	  , P.FirstName
	  , TE.ChargeCodeDate
	  , TT.Name

 END

