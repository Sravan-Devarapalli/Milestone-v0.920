CREATE PROCEDURE [dbo].[ProjectsListByProjectGroupId]
(
	@ProjectGroupId int
)
AS
BEGIN
	SELECT ProjectId,Name  
	FROM dbo.Project 
	WHERE GroupId = @ProjectGroupId
END
