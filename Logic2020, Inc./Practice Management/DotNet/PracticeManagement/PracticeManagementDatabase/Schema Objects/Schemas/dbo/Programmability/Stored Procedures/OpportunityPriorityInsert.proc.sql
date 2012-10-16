CREATE PROCEDURE [dbo].[OpportunityPriorityInsert]
@PriorityId INT,
@Description NVARCHAR(255),
@DisplayName NVARCHAR(15)
AS
BEGIN
	SET NOCOUNT ON
	UPDATE [dbo].[OpportunityPriorities]
   SET 
       [Description] =@Description      
      ,[IsInserted] = 1
	  ,[DisplayName] = @DisplayName
 WHERE id = @PriorityId
END
GO
