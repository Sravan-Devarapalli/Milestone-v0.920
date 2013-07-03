CREATE PROCEDURE [dbo].[IsUserIsOwnerOfProject]
(
	@UserLogin NVARCHAR(100),
	@Id INT,
	@IsProjectId BIT
)
AS
	DECLARE @PersonId INT
	
	SELECT @PersonId = PersonId
	FROM Person
	WHERE Alias = @UserLogin
	
	IF(@IsProjectId = 1)
	BEGIN

		IF EXISTS ( SELECT 1 FROM dbo.project AS p WHERE p.ProjectId = @Id AND ( p.ProjectOwnerId = @PersonId OR p.SalesPersonId = @PersonId))
			SELECT 'True'
		ELSE IF EXISTS ( SELECT 1 FROM dbo.ProjectManagers AS pm WHERE pm.ProjectId = @Id AND ProjectManagerId = @PersonId )
			SELECT 'True'
		ELSE
		SELECT 'False'
	END
	ELSE
	BEGIN
		IF EXISTS ( SELECT 1 FROM dbo.Milestone AS m
						INNER JOIN dbo.project AS P ON P.projectId = m.projectId
					WHERE m.MilestoneId = @Id AND (P.ProjectOwnerId = @PersonId OR p.SalesPersonId = @PersonId))
			SELECT 'True'
		ELSE IF EXISTS ( SELECT pm.ProjectManagerId  
						FROM dbo.Milestone AS milestone 
						INNER JOIN dbo.ProjectManagers AS pm ON pm.ProjectId = milestone.ProjectId
						WHERE milestone.MilestoneId = @Id 
							AND ProjectManagerId = @PersonId 
						)
			SELECT 'True'
		ELSE
		SELECT 'False'
		
	END

