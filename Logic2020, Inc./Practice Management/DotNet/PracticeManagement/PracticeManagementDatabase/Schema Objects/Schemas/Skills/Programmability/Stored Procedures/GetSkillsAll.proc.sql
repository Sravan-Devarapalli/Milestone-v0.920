CREATE PROCEDURE [skills].[GetSkillsAll]
AS
	SELECT S.SkillId,
			S.Description SkillName,
			S.SkillCategoryId,
			S.DisplayOrder
	FROM Skills.Skill S
	WHERE IsActive = 1
