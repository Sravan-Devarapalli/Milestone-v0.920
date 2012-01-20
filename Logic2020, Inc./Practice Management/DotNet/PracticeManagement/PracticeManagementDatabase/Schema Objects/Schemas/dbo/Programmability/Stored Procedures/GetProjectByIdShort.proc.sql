CREATE PROCEDURE [dbo].[GetProjectByIdShort]
(
	@ProjectId int
)
AS
BEGIN
	SELECT ProjectId,Name,ProjectNumber
	FROM [dbo].[Project]
	WHERE ProjectId = @ProjectId
END
