CREATE PROCEDURE [dbo].[GetAllTitles]
AS
BEGIN

	SELECT T.TitleId
	 , T.Title
	 , TT.TitleTypeId
	 , TT.TitleType
	 , TT.SortOrder AS TitleTypeSortOrder
	 , T.SortOrder
	 , T.PTOAccrual
	 , T.MinimumSalary
	 , T.MaximumSalary
	 , CASE WHEN EXISTS (SELECT 1 FROM Person AS p WHERE p.TitleId = T.TitleId ) OR
				 EXISTS(SELECT 1 FROM Pay AS pa	WHERE pa.TitleId = T.TitleId) 
			THEN CAST(1 AS BIT)				   	 
			ELSE CAST(0 AS BIT)
	   END AS [TitleInUse]
	FROM dbo.Title T
		INNER JOIN dbo.TitleType TT ON T.TitleTypeId = TT.TitleTypeId
	ORDER BY TT.SortOrder,T.SortOrder
END

