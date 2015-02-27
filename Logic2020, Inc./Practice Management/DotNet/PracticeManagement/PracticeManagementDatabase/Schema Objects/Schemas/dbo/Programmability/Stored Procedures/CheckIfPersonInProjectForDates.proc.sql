CREATE PROCEDURE [dbo].[CheckIfPersonInProjectForDates]
(
	@PersonId	INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN

	IF EXISTS(SELECT 1 FROM dbo.MilestonePersonEntry MPE
					   JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
					   WHERE MPE.IsBadgeRequired = 1 AND 
							(MPE.StartDate <= @EndDate AND @StartDate <= MPE.EndDate)
							AND MP.PersonId = @PersonId)
	BEGIN
			SELECT CONVERT(BIT,1) Result 
	END
	ELSE
	BEGIN
			SELECT CONVERT(BIT,0) Result
	END

END
