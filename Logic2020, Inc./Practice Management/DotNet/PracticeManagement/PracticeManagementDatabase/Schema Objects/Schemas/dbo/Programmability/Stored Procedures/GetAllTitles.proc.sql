CREATE PROCEDURE [dbo].[GetAllTitles]
AS
BEGIN

	SELECT T.TitleId,T.Title,TT.TitleTypeId,TT.TitleType,T.SortOrder,T.PTOAccrual,T.MinimumSalary,T.MaximumSalary
	FROM dbo.Title T
	INNER JOIN dbo.TitleType TT ON T.TitleTypeId = TT.TitleTypeId
	ORDER BY T.SortOrder,T.Title

END
