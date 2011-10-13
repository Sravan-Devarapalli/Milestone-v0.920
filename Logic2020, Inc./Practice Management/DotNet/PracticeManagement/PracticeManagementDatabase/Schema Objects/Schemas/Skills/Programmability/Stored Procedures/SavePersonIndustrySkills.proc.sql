CREATE PROCEDURE [Skills].[SavePersonIndustrySkills]
	@PersonId INT, 
	@IndustrySkills XML
AS
BEGIN
	
	--Delete Skill ids of no level, no experience, no lastused skills of the person.
	DELETE PIndustry
	FROM Skills.PersonIndustry PIndustry
	JOIN Skills.Industry I ON I.IndustryId = PIndustry.IndustryId
	JOIN @IndustrySkills.nodes('/Skills/IndustrySkill') t(c) ON t.c.value('@Id', 'INT') = I.IndustryId
	WHERE PIndustry.PersonId = @PersonId
		AND t.c.value('@Experience', 'INT') = 0

	--Update the level/experience/LastUsed of existing skill of the person.
	UPDATE PIndustry
	SET YearsExperience = t.c.value('@Experience', 'INT'),
		ModifiedDate = dbo.InsertingTime()
	FROM Skills.PersonIndustry PIndustry
	JOIN Skills.Industry I ON I.IndustryId = PIndustry.IndustryId
	JOIN @IndustrySkills.nodes('/Skills/IndustrySkill') t(c) ON t.c.value('@Id', 'INT') = I.IndustryId
	WHERE PIndustry.PersonId = @PersonId
		AND t.c.value('@Experience', 'INT') <> 0

	--Insert New skill for the person.
	INSERT INTO Skills.PersonIndustry(PersonId, IndustryId, YearsExperience, ModifiedDate)
	SELECT @PersonId
		, I.IndustryId
		, t.c.value('@Experience', 'INT')
		, dbo.InsertingTime()
	FROM Skills.Industry I 
	JOIN @IndustrySkills.nodes('/Skills/IndustrySkill') t(c) ON t.c.value('@Id', 'INT') = I.IndustryId
	LEFT JOIN Skills.PersonIndustry PIndustry ON I.IndustryId = PIndustry.IndustryId AND PIndustry.PersonId = @PersonId 
	WHERE PIndustry.IndustryId IS NULL
		AND t.c.value('@Experience', 'INT') <> 0

END
