CREATE PROCEDURE [dbo].[ProjectsListByProjectGroupId]
(
	@ProjectGroupId int
)
AS
BEGIN
	SELECT ProjectId
	,Name
	,ProjectNumber  
	FROM dbo.Project 
	WHERE GroupId = @ProjectGroupId
END
