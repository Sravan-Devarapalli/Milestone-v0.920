CREATE PROCEDURE [Skills].[SavePersonSkills]
	@PersonId INT, 
	@Skills XML
AS
BEGIN
	
	--Delete Skill ids of no level, no experience, no lastused skills of the person.
	DELETE PS
	FROM Skills.PersonSkill PS
	JOIN Skills.Skill S ON S.SkillId = PS.SkillId
	JOIN @Skills.nodes('/Skills/Skill') t(c) ON t.c.value('@Id', 'INT') = S.SkillId
	WHERE PS.PersonId = @PersonId
		AND t.c.value('@Level','INT') = 0
		AND t.c.value('@Experience', 'INT') = 0
		AND t.c.value('@LastUsed', 'INT') = 0



	--Update the level/experience/LastUsed of existing skill of the person.
	UPDATE PS
	SET PS.SkillLevelId = t.c.value('@Level','INT'),
		PS.YearsExperience = t.c.value('@Experience', 'INT'),
		PS.LastUsed = t.c.value('@LastUsed', 'INT'),
		PS.ModifiedDate = dbo.InsertingTime()
	FROM Skills.PersonSkill PS
	JOIN Skills.Skill S ON S.SkillId = PS.SkillId
	JOIN @Skills.nodes('/Skills/Skill') t(c) ON t.c.value('@Id', 'INT') = S.SkillId
	WHERE PS.PersonId = @PersonId
		AND NOT( t.c.value('@Level','INT') = 0
				AND t.c.value('@Experience', 'INT') = 0
				AND t.c.value('@LastUsed', 'INT') = 0 )



	--Insert New skill for the person.
	INSERT INTO Skills.PersonSkill(PersonId, SkillId, SkillLevelId, YearsExperience, LastUsed, ModifiedDate)
	SELECT @PersonId
			,S.SkillId
			,t.c.value('@Level','INT')
			,t.c.value('@Experience', 'INT')
			,t.c.value('@LastUsed', 'INT')
			,dbo.InsertingTime()
	FROM Skills.Skill S 
	JOIN @Skills.nodes('/Skills/Skill') t(c) ON t.c.value('@Id', 'INT') = S.SkillId
	LEFT JOIN Skills.PersonSkill PS ON PS.SkillId = S.SkillId AND PS.PersonId = @PersonId
	WHERE PS.SkillId IS NULL
		AND NOT( t.c.value('@Level','INT') = 0
				AND t.c.value('@Experience', 'INT') = 0
				AND t.c.value('@LastUsed', 'INT') = 0 )

END
