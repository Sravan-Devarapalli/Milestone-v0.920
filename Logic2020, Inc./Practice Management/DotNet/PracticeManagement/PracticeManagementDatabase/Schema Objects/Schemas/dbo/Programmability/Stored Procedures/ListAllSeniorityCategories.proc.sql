CREATE PROCEDURE [dbo].[ListAllSeniorityCategories]
AS
BEGIN
	SELECT  SC.SeniorityCategoryId,
			SC.Name AS SeniorityCategory
	FROM dbo.SeniorityCategory SC 
	ORDER BY SC.SeniorityCategoryId

END
