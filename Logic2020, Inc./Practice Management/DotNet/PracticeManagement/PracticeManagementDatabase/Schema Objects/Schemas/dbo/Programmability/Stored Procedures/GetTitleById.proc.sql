CREATE PROCEDURE [dbo].[GetTitleById]
(
	@TitleId		INT
)
AS
BEGIN
	SELECT T.TitleId,T.Title,TT.TitleTypeId,TT.TitleType,T.SortOrder,T.PTOAccrual,T.MinimumSalary,T.MaximumSalary
	FROM dbo.Title T
	INNER JOIN dbo.TitleType TT ON T.TitleTypeId = TT.TitleTypeId
	WHERE T.TitleId = @TitleId
END
