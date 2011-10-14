CREATE PROCEDURE [Skills].[GetPersonSkillsByPersonId]
	@PersonId INT
AS
BEGIN

	SELECT PS.SkillId
			,PS.SkillLevelId
			,PS.YearsExperience
			,PS.LastUsed
			,sc.SkillCategoryId
			,st.SkillTypeId 
	FROM [Skills].[PersonSkill] PS
	JOIN Skills.Skill s ON PS.SkillId = s.SkillId
	JOIN Skills.SkillCategory sc ON sc.SkillCategoryId = s.SkillCategoryId
	JOIN Skills.SkillType st ON st.SkillTypeId = sc.SkillTypeId
	WHERE PS.PersonId = @PersonId 
			AND s.IsActive = 1
			AND s.IsDeleted = 0

END
