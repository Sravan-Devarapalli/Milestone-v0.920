CREATE PROCEDURE [dbo].[GetPersonTitleByRange]
	(
	@PersonId	INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
	)
AS
	BEGIN
	SELECT	TitleId,
			Title 
	FROM	dbo.GetLatestPayInTheGivenRangeTable(@PersonId,@StartDate,@EndDate)
	END
