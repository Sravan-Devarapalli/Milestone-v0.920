CREATE PROCEDURE [dbo].[PersonListShortByTitleAndStatus]
	@PersonStatusIdsList NVARCHAR(50) = NULL,
	@TitleName		NVARCHAR(256) = NULL
AS
BEGIN
	DECLARE @PersonStatusIds TABLE(ID INT)
	INSERT INTO @PersonStatusIds (ID)
	SELECT ResultId 
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIdsList)

	SELECT DISTINCT p.PersonId,
					p.FirstName,
					p.LastName,
					p.IsDefaultManager
	FROM dbo.Person p
	INNER JOIN dbo.Title T ON t.TitleId = p.TitleId
	WHERE (p.PersonStatusId IN (SELECT ID FROM @PersonStatusIds) OR @PersonStatusIdsList IS NULL)
			AND (t.Title = @TitleName OR @TitleName IS NULL)
	ORDER BY LastName, FirstName
END
