CREATE PROCEDURE [dbo].[GetProjectByIdShort]
(
	@ProjectId int
)
AS
BEGIN
	SELECT ProjectId,Name,ProjectNumber,EndDate
	FROM [dbo].[Project]
	WHERE ProjectId = @ProjectId
END
