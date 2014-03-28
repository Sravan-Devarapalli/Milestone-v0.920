CREATE FUNCTION [dbo].[GetIndustriesByPerson]
(
	@PersonID	INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  
	DECLARE @Temp NVARCHAR(MAX) = ''
  
	SELECT @Temp = @Temp + I.Description+ ', '
	FROM Skills.PersonIndustry P
	JOIN Skills.Industry I ON I.IndustryId = P.IndustryId
	WHERE P.PersonId = @PersonId

	RETURN @Temp
END
