CREATE PROCEDURE [dbo].[ProjectFeedbackReport]
(
	@AccountIds	NVARCHAR(MAX) = NULL,
	@BusinessGroupIds	NVARCHAR(MAX) = NULL,
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@ProjectIds NVARCHAR(MAX) = NULL,
	@ProjectStatus NVARCHAR(MAX) = NULL,
	@ClientDirectorIds	NVARCHAR(MAX) = NULL,
	@Practices NVARCHAR(MAX) = NULL,
	@ExcludeInternalPractices BIT = 0,
	@PersonIds NVARCHAR(MAX) = NULL,
	@TitleIds  NVARCHAR(MAX) = NULL,
	@ReviewStartDateMonths NVARCHAR(MAX) = NULL,
	@ReviewEndDateMonths NVARCHAR(MAX) = NULL,
	@ProjectManagers NVARCHAR(MAX) = NULL,
	@Statuses NVARCHAR(MAX) = NULL,
	@IsExport BIT = 0,
	@PayTypeIds NVARCHAR(MAX) = NULL
)
AS
BEGIN

	DECLARE @AccountIdsTable TABLE ( Ids INT )
	DECLARE @BusinessGroupIdsTable TABLE ( Ids INT )
	DECLARE @ClientDirectorIdsTable TABLE ( Ids INT )
	DECLARE @PracticesTable TABLE ( Ids INT )
	DECLARE @PersonIdsTable TABLE ( Ids INT )
	DECLARE @TitleIdsTable TABLE ( Ids INT )
	DECLARE @ReviewStartDateMonthsTable TABLE ( Ids INT )
	DECLARE @ReviewEndDateMonthsTable TABLE ( Ids INT )
	DECLARE @ProjectManagersTable TABLE ( Ids INT )
	DECLARE @StatusesTable TABLE ( Ids INT )
	DECLARE @ProjectIdsTable TABLE ( Ids INT )
	DECLARE @ProjectStatusTable TABLE ( Ids INT )
	DECLARE @PayTypeIdsTable TABLE ( Ids INT )


	INSERT INTO @AccountIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@AccountIds)

	INSERT INTO @BusinessGroupIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@BusinessGroupIds)

	INSERT INTO @ProjectIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectIds)

	INSERT INTO @ProjectStatusTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectStatus)

	INSERT INTO @ClientDirectorIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ClientDirectorIds)

	INSERT INTO @PracticesTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@Practices)

	INSERT INTO @PersonIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PersonIds)

	INSERT INTO @TitleIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@TitleIds)
	
	INSERT INTO @ReviewStartDateMonthsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ReviewStartDateMonths)
	
	INSERT INTO @ReviewEndDateMonthsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ReviewEndDateMonths)
	
	INSERT INTO @ProjectManagersTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectManagers)
	
	INSERT INTO @StatusesTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@Statuses)

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIds)

	DECLARE @NullExists BIT = 0
	SELECT @NullExists = 1 FROM @ProjectManagersTable WHERE Ids = -1

	;WITH ProjectsForProjectManagers
	AS
	(
	    SELECT P.ProjectId
		FROM Project P
		LEFT JOIN dbo.ProjectAccess PA ON PA.ProjectId = P.ProjectId
		WHERE PA.ProjectAccessId IN (SELECT Ids FROM @ProjectManagersTable) 
			  OR (@NullExists = 1 AND PA.Id IS NULL)
	)
	SELECT DISTINCT FeedbackId,
		   PF.PersonId,
		   ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
		   P.LastName,
		   P.EmployeeNumber,
		   P.TitleId,
		   T.Title,
		   Pro.ProjectId,
		   Pro.Name AS ProjectName,
		   Pro.ProjectNumber,
		   C.ClientId,
		   C.Name AS ClientName,
		   Pro.ProjectStatusId,
		   PS.Name AS ProjectStatus,
		   PG.GroupId,
		   PG.Name AS BusinessUnit,
		   BG.BusinessGroupId,
		   BG.Name AS BusinessGroup,
		   director.PersonId AS DirectorId,
		   ISNULL(director.PreferredFirstName,director.FirstName) AS DirectorFirstName,
		   director.LastName AS DirectorLastName,
		   SM.LastName+', '+ISNULL(SM.PreferredFirstName,SM.FirstName) AS SeniorManagerName,
		   Manager.PersonId AS ProjectManagerId,
		   Manager.LastName AS ProjectManagerLastName,
		   ISNULL(Manager.PreferredFirstName,Manager.FirstName) AS ProjectManagerFirstName,
		   Pro.ProjectManagerId AS ProjectOwnerId,
		   owner.LastName ProjectOwnerLastName,
		   ISNULL(owner.PreferredFirstName,owner.FirstName) ProjectOwnerFirstName,
		   PF.ReviewPeriodStartDate AS ReviewStartDate,
		   PF.ReviewPeriodEndDate AS ReviewEndDate,
		   PFS.FeedbackStatusId,
		   PFS.Name AS FeedbackStatus,
		   StatusUpdatedBy.LastName+', '+ISNULL(StatusUpdatedBy.PreferredFirstName,StatusUpdatedBy.FirstName) AS CompletionCertificateBy,
		   PF.CompletionCertificateDate,
		   PF.CancelationReason
	FROM dbo.ProjectFeedback PF
	INNER JOIN dbo.Person P ON P.PersonId = PF.PersonId 
	INNER JOIN dbo.Title T ON T.TitleId = P.TitleId
	INNER JOIN dbo.Project Pro ON Pro.ProjectId = PF.ProjectId
	INNER JOIN dbo.Client C ON C.ClientId = Pro.ClientId
	INNER JOIN dbo.ProjectFeedbackStatus PFS ON PFS.FeedbackStatusId = PF.FeedbackStatusId
	INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = Pro.ProjectStatusId
	LEFT JOIN dbo.ProjectGroup PG ON PG.GroupId = Pro.GroupId
	LEFT JOIN dbo.BusinessGroup BG ON BG.BusinessGroupId = PG.BusinessGroupId
	LEFT JOIN dbo.Person director ON director.PersonId = Pro.ExecutiveInChargeId
	LEFT JOIN dbo.Person SM ON SM.PersonId = Pro.EngagementManagerId
	LEFT JOIN dbo.Person StatusUpdatedBy ON StatusUpdatedBy.PersonId = PF.CompletionCertificateBy
	LEFT JOIN dbo.ProjectAccess PM ON PM.ProjectId = Pro.ProjectId
	LEFT JOIN dbo.Person Manager ON Manager.PersonId = PM.ProjectAccessId
	LEFT JOIN dbo.Person owner ON owner.PersonId = Pro.ProjectManagerId
	INNER JOIN v_Pay pay ON pay.PersonId = P.PersonId AND @StartDate <= pay.EndDateOrig-1 AND pay.StartDate <= @EndDate
	WHERE (@IsExport = 1 OR (
							 @IsExport = 0 AND (@AccountIds IS NULL OR (Pro.ClientId IN (SELECT Ids FROM @AccountIdsTable)))	
							 AND (@BusinessGroupIds IS NULL OR (BG.BusinessGroupId IN (SELECT Ids FROM @BusinessGroupIdsTable)))
							 AND (@ProjectIds IS NULL OR ( PF.ProjectId IN (SELECT Ids FROM @ProjectIdsTable)))
							 AND (@ProjectStatus IS NULL OR ( Pro.ProjectStatusId IN (SELECT Ids FROM @ProjectStatusTable)))
							 AND (@ClientDirectorIds IS NULL OR (director.PersonId IN (SELECT Ids FROM @ClientDirectorIdsTable)))
							 AND (@Practices IS NULL OR (Pro.PracticeId IN (SELECT Ids FROM @PracticesTable)))
							 AND (@PersonIds IS NULL OR (PF.PersonId IN (SELECT Ids FROM @PersonIdsTable)))
							 AND (@TitleIds IS NULL OR (T.TitleId IN (SELECT Ids FROM @TitleIdsTable)))
							 AND (@ReviewStartDateMonths IS NULL OR ( DATEPART(M,PF.ReviewPeriodStartDate) IN (SELECT Ids FROM @ReviewStartDateMonthsTable)))
							 AND (@ReviewEndDateMonths IS NULL OR ( DATEPART(M,PF.ReviewPeriodEndDate) IN (SELECT Ids FROM @ReviewEndDateMonthsTable)))
							 AND (@ReviewEndDateMonths IS NULL OR ( DATEPART(M,PF.ReviewPeriodEndDate) IN (SELECT Ids FROM @ReviewEndDateMonthsTable)))
							 AND (@ProjectManagers IS NULL OR ( Pro.ProjectId IN (SELECT ProjectId FROM ProjectsForProjectManagers)))
							 AND (@Statuses IS NULL OR ( PF.FeedbackStatusId IN (SELECT Ids FROM @StatusesTable)))
							 AND (@ExcludeInternalPractices = 0 OR (@ExcludeInternalPractices = 1 AND (Pro.PracticeId NOT IN (SELECT PracticeId FROM dbo.Practice WHERE IsCompanyInternal = 1))))
							 AND (@PayTypeIds IS NULL OR (pay.Timescale IN (SELECT Ids FROM @PayTypeIdsTable)))
						   ))
		  AND PF.ReviewPeriodEndDate BETWEEN @StartDate AND @EndDate
		  AND Pro.ProjectStatusId IN (3,4)
    ORDER BY  P.LastName,
			  ISNULL(P.PreferredFirstName,P.FirstName),
			  Pro.Name,
			  PF.ReviewPeriodStartDate,
			  PF.ReviewPeriodEndDate
END
