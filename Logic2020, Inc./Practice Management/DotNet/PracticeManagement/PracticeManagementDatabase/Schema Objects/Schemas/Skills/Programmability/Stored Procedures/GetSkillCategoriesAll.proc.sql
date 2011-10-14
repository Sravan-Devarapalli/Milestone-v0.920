CREATE PROCEDURE Skills.[GetSkillCategoriesAll]
AS
	SELECT S.[Description] SkillCategoryName,
		   S.[SkillTypeId],
		   S.SkillCategoryId,
		   S.DisplayOrder
	FROM Skills.SkillCategory S
	WHERE S.IsActive = 1
		AND S.IsDeleted = 0
