CREATE PROCEDURE [dbo].[IsUserHasPermissionOnProject]
(
	@UserLogin NVARCHAR(100),
	@Id INT,
	@IsProjectId BIT = 1
)
AS
	DECLARE @PersonId INT
	DECLARE @SalespersonId INT
	DECLARE @PracticeId INT
	DECLARE @ClientId INT
	DECLARE @ProjectManagerIdsList TABLE(ProjectManagerId INT)
	DECLARE @ProjectGroup INT
	
	SELECT @PersonId = PersonId
	FROM Person
	WHERE Alias = @UserLogin

	IF(@IsProjectId != 1)
	BEGIN
	SELECT @Id = m.ProjectId FROM Milestone AS m
	WHERE m.MilestoneId = @Id
	END

	
	SELECT @SalespersonId = proj.SalesPersonId
			,@PracticeId = proj.PracticeId
			,@ClientId = proj.ClientId
			,@ProjectGroup = proj.GroupId
	FROM Project proj
	WHERE proj.ProjectId = @Id


	INSERT INTO @ProjectManagerIdsList
	SELECT pm.ProjectAccessId  
	FROM ProjectAccess AS pm 
	WHERE pm.ProjectId = @Id

	INSERT INTO @ProjectManagerIdsList
	SELECT  p.ProjectManagerId 
	FROM dbo.project AS p 
	WHERE p.ProjectId = @Id
	
	IF EXISTS (SELECT 1
				FROM aspnet_Users u
				INNER JOIN aspnet_UsersInRoles ur on ur.UserId = u.UserId
				INNER JOIN aspnet_Roles r ON r.RoleId = ur.RoleId
				WHERE u.UserName = @UserLogin AND r.LoweredRoleName = 'system administrator' )
	BEGIN
		SELECT 'True'
	END
	
	IF 4 <= (SELECT COUNT(PermissionId)
				FROM dbo.Permission permit
				WHERE PersonId = @PersonId 
					AND ((permit.TargetType = 1 AND permit.TargetId = @ClientId) 
							OR (permit.TargetType = 2 AND permit.TargetId = @ProjectGroup)
							--OR (permit.TargetType = 3 AND permit.TargetId = @SalespersonId)
							OR (permit.TargetType = 4 AND permit.TargetId IN (SELECT ProjectManagerId FROM @ProjectManagerIdsList))
							OR (permit.TargetType = 5 AND permit.TargetId = @PracticeId)
							OR (permit.TargetType IS NOT NULL AND permit.TargetId IS NULL)
						)
			)
	BEGIN
		SELECT 'True'
	END
	ELSE
	BEGIN
		SELECT 'False'
	END
GO

