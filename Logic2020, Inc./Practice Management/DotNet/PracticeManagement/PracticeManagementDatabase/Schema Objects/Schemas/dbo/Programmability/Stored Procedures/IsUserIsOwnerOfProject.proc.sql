CREATE PROCEDURE [dbo].[IsUserIsOwnerOfProject]
(
	@UserLogin NVARCHAR(100),
	@Id INT,
	@IsProjectId BIT
)
AS
	DECLARE @PersonId INT
	DECLARE @ProjectManagerIdsList TABLE(ProjectManagerId INT)
	DECLARE	@IsSalesPerson INT--as per #2914.
	
	SELECT @PersonId = PersonId
	FROM Person
	WHERE Alias = @UserLogin
	
	IF(@IsProjectId = 1)
	BEGIN
		INSERT INTO @ProjectManagerIdsList
		SELECT pm.ProjectManagerId  
		FROM ProjectManagers AS pm 
		WHERE pm.ProjectId = @Id

		IF EXISTS (SELECT 1 FROM dbo.Commission WHERE PersonId = @PersonId AND ProjectId = @Id AND CommissionType = 1)
			SET @IsSalesPerson = 1
			ELSE 
			SET @IsSalesPerson = 0
	END
	ELSE
	BEGIN
		INSERT INTO @ProjectManagerIdsList
		SELECT pm.ProjectManagerId  
		FROM Milestone AS milestone
		INNER JOIN ProjectManagers AS pm ON pm.ProjectId = milestone.ProjectId
		WHERE milestone.MilestoneId = @Id

		IF EXISTS (SELECT 1 FROM Milestone M 
					JOIN  dbo.Commission C ON  M.ProjectId = C.ProjectId
					 WHERE C.PersonId = @PersonId AND M.MilestoneId = @Id AND C.CommissionType = 1)
		SET @IsSalesPerson = 1
		ELSE 
		SET @IsSalesPerson = 0
		
	END

	IF (@IsSalesPerson =1 OR EXISTS ( SELECT 1 FROM @ProjectManagerIdsList WHERE ProjectManagerId = @PersonId ))
	BEGIN
		SELECT 'True'
	END
	ELSE
	BEGIN
		SELECT 'False'
	END
	 
	
GO
