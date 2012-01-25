CREATE PROCEDURE [dbo].[ProjectsListByProjectGroupId]
(
	@ProjectGroupId INT,
	@IsInternal		BIT
)
AS
BEGIN
	SELECT ProjectId
	,Name
	,ProjectNumber  
	FROM dbo.Project 
	WHERE GroupId = @ProjectGroupId 
		AND IsInternal = @IsInternal 
		AND ProjectStatusId IN (3,6)
		AND Name NOT IN ('PTO','HOL')
END
