-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-09-2008
-- Updated by:	
-- Update date: 
-- Description:	Selects a list of the seniorities
-- =============================================
CREATE PROCEDURE [dbo].[SeniorityListAll]
AS
	SET NOCOUNT ON

	SELECT S.SeniorityId,
			S.Name AS Seniority,
			S.SeniorityCategoryId,
			SC.Name AS SeniorityCategory
	FROM dbo.Seniority AS S
	INNER JOIN dbo.SeniorityCategory SC ON S.SeniorityCategoryId = SC.SeniorityCategoryId
	ORDER BY S.SeniorityValue,S.Name

