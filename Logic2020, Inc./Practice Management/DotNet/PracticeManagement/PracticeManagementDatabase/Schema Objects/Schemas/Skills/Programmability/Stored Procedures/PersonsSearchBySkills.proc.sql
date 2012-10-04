CREATE PROCEDURE [Skills].[PersonsSearchBySkills]
	@SkillsSearchXML XML
AS
/*
<Skills>
<Skill CatagoryId='' Id='' LevelId='' > </Skill>
<Skill CatagoryId='' Id='' LevelId='' > </Skill>
....
<Industry  Id=''> </Industry>
<Industry  Id=''> </Industry>
....
</Skills>

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @HighlightDate   DATETIME 
	SET @HighlightDate = DATEADD(M,-4,dbo.Today())
	
	--Populate the xml into table
	;WITH FilterTable
	AS
	(
		SELECT Skills.Skill.value('@CatagoryId', 'INT') AS CatagoryId,
				Skills.Skill.value('@Id', 'INT') AS SkillId,
				Skills.Skill.value('@LevelId', 'INT') AS LevelId,
				0 AS IsIndustry
		FROM @SkillsSearchXML.nodes('Skills/Skill') Skills(Skill)
		WHERE Skills.Skill.value('@CatagoryId', 'INT') != 0
		UNION ALL
		SELECT -1 AS CatagoryId,
				Skills.Skill.value('@Id', 'INT') AS SkillId,
				-1 AS LevelId,
				1 AS IsIndustry
		FROM @SkillsSearchXML.nodes('Skills/Industry') Skills(Skill)
		WHERE Skills.Skill.value('@Id', 'INT') != 0
	),
	--Cross join the populates xml table with the person table
	PersonFilterTable
	AS
	(
		SELECT P.PersonId,FT.*
		FROM dbo.Person P , FilterTable FT
		WHERE P.PersonStatusId IN (1,3,5) AND P.IsStrawman = 0
	),
	PersonSkills
	AS 
	(
		SELECT PS.PersonId,
				S.SkillCategoryId,
				PS.skillId,
				PS.SkillLevelId,
				PS.ModifiedDate,
				SL.DisplayOrder AS LevelDisplayOrder,
				PS.[YearsExperience],
				PS.[LastUsed]
		FROM Skills.PersonSkill  PS 
		INNER JOIN [Skills].Skill S ON PS.SkillId = S.SkillId 
		INNER JOIN [Skills].[SkillLevel] SL ON SL.SkillLevelId = PS.[SkillLevelId]
	),
	SkillsResult
	AS 
	(
		SELECT	PFT.PersonId,
				 PFT.CatagoryId,
				 PFT.SkillId,
				 PFT.LevelId,
				 PFT.IsIndustry,
				MAX(CASE WHEN (PS.PersonId IS NOT NULL OR [PI].PersonId IS NOT NULL) THEN 1 ELSE 0 END) AS IsFilterSatisified,
				MAX(CASE WHEN PS.ModifiedDate >= @HighlightDate OR [PI].ModifiedDate >= @HighlightDate THEN 1 ELSE 0 END) AS IsHighlighted
		FROM PersonFilterTable PFT
		LEFT JOIN PersonSkills AS PS ON  PFT.PersonId = PS.PersonId
							AND  PFT.IsIndustry = 0
							AND PFT.CatagoryId = PS.SkillCategoryId
							AND ISNULL(NULLIF(PFT.SkillId,0),PS.SkillId) = PS.SkillId
							AND ISNULL(NULLIF(PFT.LevelId,0),PS.SkillLevelId) = PS.SkillLevelId
		LEFT JOIN [Skills].PersonIndustry [PI] ON [PI].PersonId = PFT.PersonId
													AND  PFT.IsIndustry = 1
													AND PFT.SkillId = [PI].IndustryId
		GROUP BY PFT.PersonId,
				 PFT.CatagoryId,
				 PFT.SkillId,
				 PFT.LevelId,
				 PFT.IsIndustry
					
	),
	SkillsFilteredResult
	AS
	(
		SELECT SR.PersonId,
				MAX(IsHighlighted) AS IsHighlighted
		FROM SkillsResult SR 
		GROUP BY SR.PersonId
		HAVING MIN(sr.IsFilterSatisified) = 1 
	),
	SkillsFilteredResultWithLevels
	AS
	(
		SELECT	SR.PersonId,
				SR.IsHighlighted,
				ISNULL(PS.LevelDisplayOrder,0) AS LevelDisplayOrder,
				COALESCE(PS.YearsExperience,[PI].YearsExperience,0) AS YearsExperience,
				ISNULL(PS.LastUsed,0) AS LastUsed,
				RANK() OVER (PARTITION BY SR.PersonId,SR.IsHighlighted ORDER BY ISNULL(PS.LevelDisplayOrder,0) DESC,COALESCE(PS.YearsExperience,[PI].YearsExperience,0) DESC,ISNULL(PS.LastUsed,0) DESC) AS LevelOrderRank
		FROM SkillsFilteredResult SR 
		INNER JOIN PersonFilterTable PFT ON SR.PersonId = PFT.PersonId
		LEFT JOIN PersonSkills AS PS ON  PFT.PersonId = PS.PersonId
							AND  PFT.IsIndustry = 0
							AND PFT.CatagoryId = PS.SkillCategoryId
							AND ISNULL(NULLIF(PFT.SkillId,0),PS.SkillId) = PS.SkillId
							AND ISNULL(NULLIF(PFT.LevelId,0),PS.SkillLevelId) = PS.SkillLevelId
		LEFT JOIN [Skills].PersonIndustry [PI] ON [PI].PersonId = PFT.PersonId
													AND  PFT.IsIndustry = 1
													AND PFT.SkillId = [PI].IndustryId

	)
	SELECT P.PersonId,
			P.LastName,
			P.FirstName,
			PP.Id AS [ProfileId],
			PP.ProfileName,
			pp.ProfileUrl,
			SR.IsHighlighted
	FROM dbo.Person P
	INNER JOIN  SkillsFilteredResultWithLevels AS SR  ON SR.PersonId = P.PersonId AND SR.LevelOrderRank = 1
	LEFT JOIN [Skills].[PersonProfile] pp ON  P.PersonId = pp.PersonId AND PP.IsDefault = 1
	ORDER BY SR.LevelDisplayOrder DESC,
			 SR.YearsExperience DESC,
			SR.LastUsed DESC,
			P.LastName,
			P.FirstName
	
END

