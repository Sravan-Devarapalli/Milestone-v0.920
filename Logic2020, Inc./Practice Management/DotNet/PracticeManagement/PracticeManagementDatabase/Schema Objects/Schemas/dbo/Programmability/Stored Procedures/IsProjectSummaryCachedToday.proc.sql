CREATE PROCEDURE [dbo].[IsProjectSummaryCachedToday]
AS
BEGIN

	DECLARE @InsertingTime DATETIME
	SELECT	@InsertingTime = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE()))
	
	SELECT CASE WHEN COUNT(*) > 0 THEN CONVERT(BIT,1)
			ELSE CONVERT(BIT,0)
		   END AS IsCacheExistsForToday
	FROM [ProjectSummaryCache] p
	WHERE CacheDate = @InsertingTime
	
END
