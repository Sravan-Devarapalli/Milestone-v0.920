CREATE PROCEDURE [Skills].[GetPersonIndustriesByPersonId]
	@PersonId INT
AS
BEGIN

	SELECT PInd.IndustryId
		,PInd.YearsExperience
	FROM [Skills].[PersonIndustry] PInd
	JOIN Skills.Industry I ON I.IndustryId = PInd.IndustryId
	WHERE PInd.PersonId = @PersonId
		AND I.IsActive = 1
		AND I.IsDeleted = 0

END
