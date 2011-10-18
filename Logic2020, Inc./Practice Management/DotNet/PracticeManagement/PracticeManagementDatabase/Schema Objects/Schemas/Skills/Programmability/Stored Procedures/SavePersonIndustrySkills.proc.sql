CREATE PROCEDURE [Skills].[SavePersonIndustrySkills]
	@PersonId		INT, 
	@IndustrySkills XML,
	@UserLogin      NVARCHAR(255)
AS
BEGIN
	
	/* --@IndustrySkills XML Format

	<Skills>
		<IndustrySkill Id="" Experience=""/>
	</Skills>

	*/
		
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	BEGIN TRY
		BEGIN TRANSACTION PersonIndustrySave
	
		--Delete Skill ids of no level, no experience, no lastused skills of the person.
		DELETE PIndustry
		FROM Skills.PersonIndustry PIndustry
		JOIN Skills.Industry I ON I.IndustryId = PIndustry.IndustryId
		JOIN @IndustrySkills.nodes('/Skills/IndustrySkill') t(c) ON t.c.value('@Id', 'INT') = I.IndustryId
		WHERE PIndustry.PersonId = @PersonId
			AND t.c.value('@Experience', 'INT') = 0

					
		IF NOT EXISTS (SELECT 1 FROM dbo.SessionLogData WHERE SessionID = @@SPID AND UserLogin = @UserLogin)
		BEGIN
			-- Start logging session
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		END


		--Update the level/experience/LastUsed of existing skill of the person.
		UPDATE PIndustry
		SET YearsExperience = t.c.value('@Experience', 'INT'),
			ModifiedDate = dbo.InsertingTime()
		FROM Skills.PersonIndustry PIndustry
		JOIN Skills.Industry I ON I.IndustryId = PIndustry.IndustryId
		JOIN @IndustrySkills.nodes('/Skills/IndustrySkill') t(c) ON t.c.value('@Id', 'INT') = I.IndustryId
		WHERE PIndustry.PersonId = @PersonId
			AND t.c.value('@Experience', 'INT') <> 0
			AND PIndustry.YearsExperience <> t.c.value('@Experience', 'INT')

					
		IF NOT EXISTS (SELECT 1 FROM dbo.SessionLogData WHERE SessionID = @@SPID AND UserLogin = @UserLogin)
		BEGIN
			-- Start logging session
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		END


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
		
		COMMIT TRANSACTION PersonIndustrySave
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION PersonIndustrySave

		DECLARE @Error NVARCHAR(MAX)
		SEt @Error = ERROR_MESSAGE()

		RAISERROR(@Error, 16, 1)
	END CATCH
	
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
