CREATE PROCEDURE [dbo].[CSATSummaryReport]
(
	@StartDate		DATETIME,
	@EndDate		DATETIME,
    @PracticeIds	VARCHAR(MAX) = NULL,
    @AccountIds		VARCHAR(MAX) = NULL,
	@IsExport       BIT = 0 
)
AS
BEGIN
     DECLARE @PracticeIdsTable TABLE ( Ids INT )
	 DECLARE @AccountIdsTable TABLE ( Ids INT )
	 SELECT @StartDate = CONVERT(DATE,@StartDate),@EndDate = CONVERT(DATE,@EndDate)

	 INSERT INTO @PracticeIdsTable(Ids)
	 SELECT ResultId
   	 FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	 INSERT INTO @AccountIdsTable(Ids)
	 SELECT ResultId
	 FROM dbo.ConvertStringListIntoTable(@AccountIds)

	  ;WITH ProjectsRecentlyUpdatedCSATS AS 
	 (
		  SELECT PC.ProjectId, MAX(ModifiedDate) AS ModifiedDate,COUNT(*) AS NumberOfCSATs
		  FROM dbo.ProjectCSAT PC
		  INNER JOIN dbo.Project P ON pc.ProjectId=p.ProjectId AND P.ProjectStatusId IN (3,4)
		  WHERE PC.CompletionDate BETWEEN @StartDate AND @EndDate
		        AND	(
						@PracticeIds IS NULL
						OR P.PracticeId IN ( SELECT Ids FROM  @PracticeIdsTable)
					) 
				AND (
							@AccountIds IS NULL
							OR P.ClientId IN ( SELECT Ids FROM  @AccountIdsTable)
					) 
		  GROUP BY PC.ProjectId
	 ),
	 EstimatedRevenueByProject
	 AS 
	 (SELECT r.ProjectId,
	      SUM( CASE
	           WHEN r.IsHourlyAmount = 1 OR r.HoursPerDay = 0
	           THEN ISNULL(m.Amount*m.HoursPerDay, 0)
	           ELSE ISNULL(r.MilestoneDailyAmount * m.HoursPerDay / r.HoursPerDay, r.MilestoneDailyAmount)
	       END) AS EstimatedRevenue
	   
	  FROM   dbo.v_MilestoneRevenueRetrospective AS r 
	       INNER JOIN dbo.v_MilestonePersonSchedule m ON m.MilestoneId = r.MilestoneId AND m.Date = r.Date
	GROUP BY r.ProjectId
	 ),
	 RecentProjectCompletedStatus AS
	 (
	     SELECT MAX(PSH.StartDate) AS CompletedStatusDate,PSH.ProjectId
		 FROM dbo.ProjectStatusHistory PSH
		 WHERE PSH.ProjectStatusId = 4
		 GROUP BY PSH.ProjectId 
	 ) 
		SELECT	P.ProjectId,
				C.Name AS Account,
				BG.Name AS BusinessGroupName,
				PG.Name AS BusinessUnitName,
				P.ProjectNumber,
				P.Name AS ProjectName,
				PS.Name AS ProjectStatusName,
				PR.Name AS PracticeAreaName,
				ERP.EstimatedRevenue AS SowBudget,
				PCSAT.ReferralScore,
				PCSAT.CSATId,
				CASE WHEN PRC.NumberOfCSATs > 1 THEN 1 ELSE 0 END AS HasMultipleCSATs,
				Per1.LastName AS ProjectOwnerLastName,
				Per1.FirstName AS ProjectOwnerFirstName,
				CASE WHEN ERP.EstimatedRevenue >= 50000 THEN 1 ELSE 0 END AS CSATEligible,
				P.StartDate,
				P.EndDate,
				RPCS.CompletedStatusDate,
				Per3.LastName+', '+Per3.FirstName AS SalesPerson,
				Per2.LastName AS DirectorLastName,
				Per2.FirstName AS DirectorFirstName,
				dbo.GetProjectManagerNames(P.ProjectId) AS ProjectManagers,
				CSATOwner.LastName+', '+CSATOwner.FirstName AS CSATOwnerName,
				PCSAT.CompletionDate,
				PCSAT.ReviewStartDate,
				PCSAT.ReviewEndDate,
			    CSATReviewer.LastName+', '+CSATReviewer.FirstName AS CSATReviewer,
				PCSAT.Comments
		FROM Project P
		INNER JOIN dbo.Client C ON C.ClientId = P.ClientId AND P.ProjectStatusId IN (3,4) AND p.IsAllowedToShow =1
		INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = P.GroupId 
		INNER JOIN dbo.BusinessGroup BG ON BG.BusinessGroupId = PG.BusinessGroupId
		INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = P.ProjectStatusId
		INNER JOIN dbo.Practice PR ON PR.PracticeId = P.PracticeId
		INNER JOIN dbo.Commission Comm ON Comm.ProjectId = P.ProjectId AND Comm.CommissionType = 1 
		INNER JOIN dbo.Person Per3 ON Per3.PersonId = Comm.PersonId
		INNER JOIN dbo.Person Per1 ON Per1.PersonId = P.ProjectOwnerId
		LEFT JOIN EstimatedRevenueByProject ERP ON ERP.ProjectId = P.ProjectId
		LEFT JOIN dbo.Person Per2 ON Per2.PersonId = P.DirectorId
		LEFT JOIN dbo.Person CSATOwner ON CSATOwner.PersonId = P.ReviewerId
		LEFT JOIN RecentProjectCompletedStatus RPCS ON RPCS.ProjectId = P.ProjectId 
		LEFT JOIN ProjectsRecentlyUpdatedCSATS PRC ON P.ProjectId = PRC.ProjectId 
		LEFT JOIN dbo.ProjectCSAT PCSAT ON PCSAT.ProjectId = P.ProjectId  
		LEFT JOIN dbo.Person CSATReviewer ON CSATReviewer.PersonId = PCSAT.ReviewerId
		WHERE (P.ProjectId = PRC.ProjectId OR @IsExport = 1) 
			AND ( @IsExport = 1 OR PCSAT.ModifiedDate = PRC.ModifiedDate)  
			AND (@IsExport = 0 OR ((P.EndDate >= @StartDate AND P.StartDate <= @EndDate) OR (P.StartDate IS NULL AND P.EndDate IS NULL)))
		ORDER BY p.ProjectNumber ASC,PCSAT.CompletionDate DESC
END

