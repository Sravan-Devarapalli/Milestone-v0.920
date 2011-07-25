CREATE PROCEDURE [dbo].[IsUserIsOwnerOfProject]
(
	@UserLogin NVARCHAR(100),
	@Id INT,
	@IsProjectId BIT
)
AS
	DECLARE @PersonId INT
	DECLARE @OwnerId INT
	
	SELECT @PersonId = PersonId
	FROM Person
	WHERE Alias = @UserLogin
	
	IF(@IsProjectId = 1)
	BEGIN
	SELECT @OwnerId = proj.ProjectManagerId  
	FROM v_Project AS proj
	WHERE proj.ProjectId = @Id
	END
	ELSE
	BEGIN
		SELECT @OwnerId = milestone.ProjectManagerId 
		FROM v_Milestone AS milestone
		WHERE milestone.MilestoneId = @Id
	END

	IF(@PersonId = @OwnerId)
	BEGIN
		SELECT 'True'
	END
	ELSE
	BEGIN
		SELECT 'False'
	END
	 
	
GO
