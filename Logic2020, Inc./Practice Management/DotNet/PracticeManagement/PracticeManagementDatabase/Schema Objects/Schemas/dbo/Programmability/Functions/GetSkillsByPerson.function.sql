CREATE FUNCTION [dbo].[GetSkillsByPerson]
(
	@PersonID	INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  
	DECLARE @Temp NVARCHAR(MAX) = ''
  
	SELECT @Temp = @Temp + S.Description+ ', '
	FROM Skills.PersonSkill PS
	JOIN Skills.Skill S ON S.SkillId = PS.SkillId
	WHERE PS.PersonId = @PersonId

	RETURN @Temp
END
