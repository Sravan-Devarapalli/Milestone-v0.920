CREATE PROCEDURE [dbo].[OpportunityPriorityDelete]
@UpdatedPriorityId INT,
@DeletedPriorityId INT
AS
BEGIN
	SET NOCOUNT ON

	IF(@UpdatedPriorityId IS NOT NULL)
	BEGIN 
		UPDATE v_Opportunity
		SET PriorityId =@UpdatedPriorityId
		WHERE PriorityId =@DeletedPriorityId
		
		UPDATE [dbo].[OpportunityPriorities]
		SET [IsInserted] = 1    
		WHERE id = @UpdatedPriorityId
	END
	
	UPDATE [dbo].[OpportunityPriorities]
	SET [Description] =NULL  
	   ,[IsInserted] = 0    
	WHERE id = @DeletedPriorityId
END

GO
