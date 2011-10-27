CREATE PROCEDURE [skills].[GetSkillTypesAll]
	 
AS
	SELECT S.[Description] SkillTypeDescription,
		   S.[SkillTypeId],
		   S.DisplayOrder
	FROM Skills.SkillType S
	order by s.DisplayOrder;
