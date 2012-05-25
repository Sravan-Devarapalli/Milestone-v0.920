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
		@EndDateLocal DATETIME

	SET @StartDateLocal = CONVERT(DATE, @StartDate)
	SET @EndDateLocal = CONVERT(DATE, @EndDate)

	SELECT PG.Name AS GroupName,
			PG.Active AS GroupStatus,
			P.FirstName,
			P.LastName,
			TE.ChargeCodeDate,
			TEH.ActualHours,
			TE.Note
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId 
                                                AND TE.ChargeCodeDate BETWEEN @StartDateLocal
                                                AND @EndDateLocal
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND CC.TimeEntrySectionId = 2--Here 2 is Business Develpment section.
	INNER JOIN dbo.Client C ON C.ClientId = CC.ClientId
	INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId
	INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId AND TE.ChargeCodeDate <= ISNULL(P.TerminationDate, dbo.GetFutureDate())
	INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
	WHERE C.ClientId = @AccountId
		AND ( @BusinessUnitIds IS NULL
				OR PG.GroupId IN (SELECT ResultId
									FROM dbo.ConvertStringListIntoTable(@BusinessUnitIds)
									)
			)

END
