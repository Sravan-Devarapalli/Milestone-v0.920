CREATE PROCEDURE [skills].[GetSkillLevelsAll]
	 
AS
	SELECT S.[Description] SkillLevelName,
		   S.[SkillLevelId],
		   S.DisplayOrder,
		   s.[Definition] SkillLevelDefinition
	FROM Skills.SkillLevel S
	WHERE S.IsActive = 1
		AND S.IsDeleted = 0
	 
