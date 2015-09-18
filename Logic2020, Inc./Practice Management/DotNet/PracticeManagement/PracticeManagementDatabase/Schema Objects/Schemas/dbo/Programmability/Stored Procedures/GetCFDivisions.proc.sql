CREATE PROCEDURE [dbo].[GetCFDivisions]
AS
BEGIN

	SELECT	D.DivisionId,
			D.DivisionCode,
			D.DivisionName,
			D.ParentId,
			Ownr.DivisionCode AS ParentDivisionCode,
			Ownr.DivisionName AS ParentDivisionName
	FROM dbo.Division_CF D
	LEFT JOIN dbo.Division_CF Ownr ON Ownr.DivisionId = D.ParentId

END
