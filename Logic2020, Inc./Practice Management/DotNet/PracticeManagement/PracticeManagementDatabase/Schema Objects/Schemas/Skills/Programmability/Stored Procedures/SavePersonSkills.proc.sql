CREATE PROCEDURE [Skills].[SavePersonSkills]
	@PersonId	INT, 
	@Skills		XML,
	@UserLogin  NVARCHAR(255)
AS
BEGIN
	
	/*--@Skills XML Format

	<Skills>
		<Skill Id="" Level="" Experience="" LastUsed="" />
	</Skills>

	*/

	
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	
	
	BEGIN TRY
		BEGIN TRANSACTION PersonIndustrySave

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
			AND ( PS.SkillLevelId <> t.c.value('@Level','INT')
					OR PS.YearsExperience <> t.c.value('@Experience', 'INT')
					OR PS.LastUsed <> t.c.value('@LastUsed', 'INT')
				)



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
	
		COMMIT TRANSACTION PersonSkillsSave
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION PersonSkillsSave

		DECLARE @Error NVARCHAR(MAX)
		SEt @Error = ERROR_MESSAGE()

		RAISERROR(@Error, 16, 1)
	END CATCH
	
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
