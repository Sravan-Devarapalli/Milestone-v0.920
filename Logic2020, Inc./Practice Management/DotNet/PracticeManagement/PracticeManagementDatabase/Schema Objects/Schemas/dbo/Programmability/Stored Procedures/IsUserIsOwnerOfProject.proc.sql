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

		IF EXISTS ( SELECT 1 FROM dbo.project AS p WHERE p.ProjectId = @Id AND ( p.ProjectManagerId = @PersonId OR p.SalesPersonId = @PersonId))
			SELECT 'True'
		ELSE IF EXISTS ( SELECT 1 FROM dbo.ProjectAccess AS pm WHERE pm.ProjectId = @Id AND ProjectAccessId = @PersonId )
			SELECT 'True'
		ELSE
		SELECT 'False'
	END
	ELSE
	BEGIN
		IF EXISTS ( SELECT 1 FROM dbo.Milestone AS m
						INNER JOIN dbo.project AS P ON P.projectId = m.projectId
					WHERE m.MilestoneId = @Id AND (P.ProjectManagerId = @PersonId OR p.SalesPersonId = @PersonId))
			SELECT 'True'
		ELSE IF EXISTS ( SELECT pm.ProjectAccessId  
						FROM dbo.Milestone AS milestone 
						INNER JOIN dbo.ProjectAccess AS pm ON pm.ProjectId = milestone.ProjectId
						WHERE milestone.MilestoneId = @Id 
							AND ProjectAccessId = @PersonId 
						)
			SELECT 'True'
		ELSE
		SELECT 'False'
		
	END

