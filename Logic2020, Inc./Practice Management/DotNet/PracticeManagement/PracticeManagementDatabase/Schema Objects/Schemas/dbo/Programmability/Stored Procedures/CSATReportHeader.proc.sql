CREATE PROCEDURE [dbo].[CSATReportHeader]
(
  @StartDate	DATETIME,
  @EndDate		DATETIME,
  @PracticeIds	VARCHAR(MAX) = NULL,
  @AccountIds	VARCHAR(MAX) = NULL
)
AS
BEGIN
	 DECLARE @PracticeIdsTable TABLE ( Id INT )
	 DECLARE @AccountIdsTable TABLE ( Id INT )
	 SELECT @StartDate = CONVERT(DATE,@StartDate),@EndDate = CONVERT(DATE,@EndDate)

	 INSERT INTO @PracticeIdsTable(Id)
	 SELECT ResultId
   	 FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	 INSERT INTO @AccountIdsTable(Id)
	 SELECT ResultId
	 FROM dbo.ConvertStringListIntoTable(@AccountIds)

   
    ;WITH ProjectsRecentlyUpdatedCSATS AS 
	 (
		  SELECT PC.ProjectId, 
				MAX(CASE WHEN PC.ReferralScore <> -1 THEN ModifiedDate END) AS ModifiedDateWithoutFilter,
				MAX( CASE WHEN (@PracticeIds IS null OR  pra.Id IS NOT NULL ) AND ( @AccountIds IS NULL OR acc.Id IS NOT NULL) AND (PC.ReferralScore <> -1) THEN  ModifiedDate END) AS ModifiedDateWithFilter
		  FROM dbo.ProjectCSAT PC
		  INNER JOIN dbo.Project P ON pc.ProjectId=p.ProjectId AND P.ProjectStatusId IN (3,4)
		  LEFT JOIN @PracticeIdsTable pra ON pra.Id = P.PracticeId
		  LEFT JOIN @AccountIdsTable acc ON acc.Id = P.ClientId
		  WHERE PC.CompletionDate BETWEEN @StartDate AND @EndDate
		  GROUP BY PC.ProjectId
	 )
 
	 SELECT 
		SUM(CASE WHEN PC.ReferralScore =10 OR PC.ReferralScore=9 THEN 1 ELSE 0 END) AS PromotersWithoutFilter,
		SUM(CASE WHEN PC.ReferralScore =8 OR PC.ReferralScore=7 THEN 1 ELSE 0 END) AS PassivesWithoutFilter,
		SUM(CASE WHEN PC.ReferralScore BETWEEN 0 AND 6 THEN 1 ELSE 0 END) AS DetractorsWithoutFilter,
		SUM(CASE WHEN PCs.ReferralScore =10 OR PCs.ReferralScore=9 THEN 1 ELSE 0 END) AS PromotersWithFilter,
		SUM(CASE WHEN PCs.ReferralScore =8 OR PCs.ReferralScore=7 THEN 1 ELSE 0 END) AS PassivesWithFilter,
		SUM(CASE WHEN PCs.ReferralScore BETWEEN 0 AND 6 THEN 1 ELSE 0 END) AS DetractorsWithFilter  
	 FROM ProjectsRecentlyUpdatedCSATS  PRC 
	 LEFT  JOIN dbo.ProjectCSAT PC ON PC.ProjectId = PRC.ProjectId AND PC.ModifiedDate = PRC.ModifiedDateWithoutFilter
	 LEFT JOIN dbo.ProjectCSAT PCS ON PCS.ProjectId = PRC.ProjectId AND PCS.ModifiedDate = PRC.ModifiedDateWithFilter
	 WHERE PC.ReferralScore <> -1 AND (PCS.CSATId IS NULL OR PCS.ReferralScore <> -1)

END

