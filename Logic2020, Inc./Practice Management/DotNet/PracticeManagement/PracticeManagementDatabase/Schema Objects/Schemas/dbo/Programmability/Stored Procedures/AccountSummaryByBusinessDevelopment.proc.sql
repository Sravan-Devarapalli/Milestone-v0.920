CREATE PROCEDURE [dbo].[AccountSummaryByBusinessDevelopment]
(
	@AccountId	INT,
	@BusinessUnitIds	NVARCHAR(MAX) = NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN
	
	DECLARE @StartDateLocal DATETIME ,
		@EndDateLocal DATETIME,
		@AccountIdLocal INT,
		@BusinessUnitIdsLocal	NVARCHAR(MAX)

SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
	 , @EndDateLocal = CONVERT(DATE, @EndDate)
	 , @AccountIdLocal = @AccountId
	 , @BusinessUnitIdsLocal = @BusinessUnitIds

SELECT PG.GroupId AS [BusinessUnitId]
	 , PG.Name AS [BusinessUnitName]
	 , PG.Active
	 , TT.Name AS [TimeTypeName]
	 , P.PersonId
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
		ON P.PersonId = TE.PersonId AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate, dbo.GetFutureDate())

WHERE
	CC.ClientId = @AccountIdLocal
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
