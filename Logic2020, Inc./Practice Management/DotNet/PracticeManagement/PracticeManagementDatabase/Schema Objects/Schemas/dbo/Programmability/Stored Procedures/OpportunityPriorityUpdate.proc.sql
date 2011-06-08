CREATE PROCEDURE [dbo].[OpportunityPriorityUpdate]
@OldPriorityId INT,
@PriorityId INT,
@Description NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON

	IF(@OldPriorityId = @PriorityId)
	BEGIN
		UPDATE [dbo].[OpportunityPriorities]
		SET 
		   [Description] =@Description      
		WHERE id = @PriorityId
	END
	ELSE
	BEGIN
		UPDATE [dbo].[OpportunityPriorities]
		SET 
		   [Description] =@Description  
		   ,[IsInserted] = 1    
		WHERE id = @PriorityId
		
		UPDATE [dbo].[OpportunityPriorities]
		SET 
		   [Description] =NULL  
		   ,[IsInserted] = 0    
		WHERE id = @OldPriorityId
		
		UPDATE v_Opportunity
		SET PriorityId =@PriorityId
		WHERE PriorityId =@OldPriorityId
		
	END
END
GO
