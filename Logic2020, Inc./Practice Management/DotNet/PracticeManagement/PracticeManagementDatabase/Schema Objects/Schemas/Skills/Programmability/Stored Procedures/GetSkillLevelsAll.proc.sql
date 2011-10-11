CREATE PROCEDURE [skills].[GetSkillLevelsAll]
	 
AS
	SELECT S.[Description] SkillLevelName,
		   S.[SkillLevelId],
		   S.DisplayOrder
	FROM Skills.SkillLevel S
	WHERE IsActive = 1
	 
