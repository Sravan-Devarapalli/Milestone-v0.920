CREATE PROCEDURE [dbo].[RecruitingMetricsDelete]
(
	@RecruitingMetricsId INT
)
AS
BEGIN

	DELETE RecruitingMetrics
	WHERE RecruitingMetricsId = @RecruitingMetricsId

END
