CREATE PROCEDURE [dbo].[IsUserIsOwnerOfProject]
(
	@UserLogin NVARCHAR(100),
	@Id INT,
	@IsProjectId BIT
)
AS
	DECLARE @PersonId INT
	DECLARE @OwnerId INT,
			@IsSalesPerson INT--as per #2914.
	
	SELECT @PersonId = PersonId
	FROM Person
	WHERE Alias = @UserLogin
	
	IF(@IsProjectId = 1)
	BEGIN
		SELECT @OwnerId = proj.ProjectManagerId  
		FROM v_Project AS proj
		WHERE proj.ProjectId = @Id
		IF EXISTS (SELECT 1 FROM dbo.Commission WHERE PersonId = @PersonId AND ProjectId = @Id AND CommissionType = 1)
			SET @IsSalesPerson = 1
			ELSE 
			SET @IsSalesPerson = 0
	END
	ELSE
	BEGIN
		SELECT @OwnerId = milestone.ProjectManagerId 
		FROM v_Milestone AS milestone
		WHERE milestone.MilestoneId = @Id

		IF EXISTS (SELECT 1 FROM Milestone M 
					JOIN  dbo.Commission C ON  M.ProjectId = C.ProjectId
					 WHERE C.PersonId = @PersonId AND M.MilestoneId = @Id AND C.CommissionType = 1)
		SET @IsSalesPerson = 1
		ELSE 
		SET @IsSalesPerson = 0
		
	END

	IF(@PersonId = @OwnerId OR @IsSalesPerson =1 )
	BEGIN
		SELECT 'True'
	END
	ELSE
	BEGIN
		SELECT 'False'
	END
	 
	
GO
