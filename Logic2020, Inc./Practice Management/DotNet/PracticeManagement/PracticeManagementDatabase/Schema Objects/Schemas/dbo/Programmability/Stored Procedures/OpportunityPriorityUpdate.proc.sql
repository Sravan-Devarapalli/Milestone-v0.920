CREATE PROCEDURE [dbo].[OpportunityPriorityUpdate]
@OldPriorityId INT,
@PriorityId INT,
@Description NVARCHAR(255),
@DisplayName NVARCHAR(15),
@UserLogin          NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON
	-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	IF(@OldPriorityId = @PriorityId)
	BEGIN
		UPDATE [dbo].[OpportunityPriorities]
		SET 
		   [Description] =@Description,
		   [DisplayName] = @DisplayName
		WHERE id = @PriorityId
	END
	ELSE
	BEGIN
		UPDATE [dbo].[OpportunityPriorities]
		SET 
		   [Description] =@Description 
		   ,[DisplayName] = @DisplayName 
		   ,[IsInserted] = 1    
		WHERE id = @PriorityId
		
		UPDATE [dbo].[OpportunityPriorities]
		SET 
		   [Description] =NULL  
		   ,[DisplayName] = NULL
		   ,[IsInserted] = 0    
		WHERE id = @OldPriorityId
		
		UPDATE Opportunity
		SET PriorityId =@PriorityId
		WHERE PriorityId =@OldPriorityId
		
	END

	-- End logging session
		EXEC dbo.SessionLogUnprepare
END
GO
