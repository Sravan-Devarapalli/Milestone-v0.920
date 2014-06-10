CREATE PROCEDURE [dbo].[GetAllFeedbackStatuses]
AS
BEGIN
 
   SELECT FeedbackStatusId,
		  Name
   FROM dbo.ProjectFeedbackStatus 
  
END
