CREATE PROCEDURE [dbo].[SaveRecruitingMetrics]
(
	@RecruitingMetricsId	INT,
	@Name					NVARCHAR(50),
	@SortOrder				INT
)
AS
BEGIN
	UPDATE RecruitingMetrics
	SET Name = @Name,
		SortOrder = @SortOrder
	WHERE RecruitingMetricsId = @RecruitingMetricsId
END
	
