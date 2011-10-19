﻿CREATE PROCEDURE [Skills].[PersonsSearchBySkillsText]
	@SkillsSearchText NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SkillsXML XML,@Query NVARCHAR(MAX)

	SELECT @SkillsSearchText = REPLACE(REPLACE(REPLACE(REPLACE(@SkillsSearchText,'_','[_]'),'%','[%]'),'''',''''''),',','</Skill><Skill>')

	SELECT @SkillsXML = '<Skills><Skill>'+@SkillsSearchText+'</Skill></Skills>'

	 

	DECLARE @SearchTerms TABLE
	(
		Term NVARCHAR(MAX)
	)

	INSERT INTO @SearchTerms
	SELECT   t.c.value('.', 'NVARCHAR(MAX)') 
	FROM @SkillsXML.nodes('/Skills/Skill') t(c)



	SELECT @Query= ISNULL(@Query+' OR','WHERE ')+'
	S.[Description]  LIKE ''%'+ RTRIM(LTRIM(Term))+ '%'''
	FROM @SearchTerms
	WHERE Term <> ''

	SELECT @Query= @Query +'
	OR I.[Description]  LIKE ''%'+ RTRIM(LTRIM(Term))+ '%'''
	FROM @SearchTerms
	WHERE Term <> ''

	SELECT @Query ='
	SELECT DISTINCT P.PersonId,
			P.LastName,
			P.FirstName
	FROM Person P
	LEFT JOIN Skills.PersonSkill  PS ON PS.PersonId = P.PersonId
	LEFT JOIN [skills].Skill S ON PS.SkillId = S.SkillId 
	LEFT JOIN [skills].PersonIndustry [PI] ON [PI].PersonId = P.PersonId
	LEFT JOIN [skills].Industry I ON I.IndustryId = [PI].IndustryId
	'+@Query 
	 

	EXEC SP_EXECUTESQL @Query
END
