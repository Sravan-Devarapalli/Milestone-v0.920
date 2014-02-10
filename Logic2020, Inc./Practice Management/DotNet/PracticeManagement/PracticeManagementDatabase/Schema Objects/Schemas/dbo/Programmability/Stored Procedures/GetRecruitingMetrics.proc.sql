CREATE PROCEDURE [dbo].[GetRecruitingMetrics]
(
	@RecruitingMetricsTypeId	INT = NULL
)
AS
BEGIN
	
	SELECT  RM.RecruitingMetricsId,
			RM.Name AS RecruitingMetrics,
			RM.RecruitingMetricsTypeId,
			RM.SortOrder AS SortOrder,
			CASE WHEN EXISTS (SELECT 1 FROM Person AS p WHERE (@RecruitingMetricsTypeId = 1 AND p.SourceId = RM.RecruitingMetricsId) OR (@RecruitingMetricsTypeId = 2 AND p.TargetedCompanyId = RM.RecruitingMetricsId) OR (@RecruitingMetricsTypeId IS NULL AND (p.SourceId = RM.RecruitingMetricsId OR p.TargetedCompanyId = RM.RecruitingMetricsId)))
			THEN CAST(1 AS BIT)				   	 
			ELSE CAST(0 AS BIT)
	   END AS [RecruitingMetricsInUse]
	FROM RecruitingMetrics AS RM
	WHERE @RecruitingMetricsTypeId IS NULL OR (RM.RecruitingMetricsTypeId = @RecruitingMetricsTypeId)
	ORDER BY RM.SortOrder

END
