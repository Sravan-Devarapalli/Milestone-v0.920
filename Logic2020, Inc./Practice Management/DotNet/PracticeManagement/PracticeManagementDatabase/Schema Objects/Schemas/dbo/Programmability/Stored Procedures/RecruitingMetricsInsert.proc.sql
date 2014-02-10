CREATE PROCEDURE [dbo].[RecruitingMetricsInsert]
(
	@Name						NVARCHAR(50),
	@RecruitingMetricsTypeId	INT,
	@SortOrder					INT
)
AS
BEGIN

	INSERT INTO RecruitingMetrics(Name,RecruitingMetricsTypeId,SortOrder)
	VALUES (@Name,@RecruitingMetricsTypeId,@SortOrder)

END
