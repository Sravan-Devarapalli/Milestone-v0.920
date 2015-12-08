CREATE PROCEDURE [dbo].[PersonListShortByTitleAndStatus]
	@PersonStatusIdsList NVARCHAR(50) = NULL,
	@TitleNames		NVARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @PersonStatusIds TABLE(ID INT)
	INSERT INTO @PersonStatusIds (ID)
	SELECT ResultId 
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIdsList)

	DECLARE @PersonTitleNames TABLE (TitleName NVARCHAR(MAX)) 
	INSERT INTO @PersonTitleNames (TitleName)
	SELECT ResultName
	FROM dbo.ConvertStringListIntoStringTable(@TitleNames)

	SELECT DISTINCT p.PersonId,
					p.FirstName,
					p.LastName,
					p.IsDefaultManager
	FROM dbo.Person p
	INNER JOIN dbo.Title T ON t.TitleId = p.TitleId
	WHERE (p.PersonStatusId IN (SELECT ID FROM @PersonStatusIds) OR @PersonStatusIdsList IS NULL)
			AND (t.Title IN (SELECT TitleName FROM @PersonTitleNames)OR @TitleNames IS NULL)
	ORDER BY LastName, FirstName
END

