CREATE PROCEDURE [dbo].[OpportunityPriorityInsert]
@PriorityId INT,
@Description NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON
	UPDATE [dbo].[OpportunityPriorities]
   SET 
       [Description] =@Description      
      ,[IsInserted] = 1
 WHERE id = @PriorityId
END
GO
