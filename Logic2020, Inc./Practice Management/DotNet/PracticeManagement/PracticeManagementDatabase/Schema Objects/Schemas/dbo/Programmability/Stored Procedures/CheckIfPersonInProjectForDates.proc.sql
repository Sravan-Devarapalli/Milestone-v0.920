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
					   JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
					   JOIN dbo.Project P ON P.ProjectId = M.ProjectId
					   WHERE MPE.IsBadgeRequired = 1 
							AND (MPE.BadgeStartDate <= @EndDate AND @StartDate <= MPE.BadgeEndDate)
							AND MP.PersonId = @PersonId 
							AND P.ProjectStatusId IN (1,2,3,4))
	BEGIN
			SELECT CONVERT(BIT,1) Result 
	END
	ELSE
	BEGIN
			SELECT CONVERT(BIT,0) Result
	END

END
