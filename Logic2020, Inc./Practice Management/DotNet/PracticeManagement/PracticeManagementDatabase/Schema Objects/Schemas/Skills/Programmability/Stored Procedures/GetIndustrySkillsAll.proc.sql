CREATE PROCEDURE [Skills].[GetIndustrySkillsAll]
AS
BEGIN
	SELECT I.IndustryId 'IndustryId' ,
			I.Description 'IndustryName',
			I.DisplayOrder
	FROM Skills.Industry I
	WHERE I.IsActive = 1 AND I.IsDeleted = 0
END
