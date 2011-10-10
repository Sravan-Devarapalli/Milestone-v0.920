CREATE PROCEDURE Skills.[GetSkillCategoriesAll]
AS
	SELECT S.[Description] SkillCategoryName,
		   S.[SkillTypeId],
		   S.SkillCategoryId,
		   S.DisplayOrder
	FROM Skills.SkillCategory S
	WHERE IsActive = 1
