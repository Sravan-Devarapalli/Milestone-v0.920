CREATE PROCEDURE [dbo].[CheckIfPersonInProjectsForThisPeriod]
(
	@ModifiedEndDate	DATETIME, --LESS THAN OLDENDDATE
	@OldEndDate			DATETIME,
	@ModifiedStartDate	DATETIME, -- GREATER THAN OLDSTARTDATE
	@OldStartDate		DATETIME,
	@PersonId			INT
)
AS
BEGIN
	
	DECLARE @ProjectsExists	BIT = 0

	IF(@ModifiedEndDate IS NOT NULL)
	BEGIN
		
		SELECT @ProjectsExists = 1 
		FROM dbo.MilestonePersonEntry MPE 
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
		WHERE MP.PersonId = @PersonId AND P.ProjectStatusId IN (1,2,3,4) AND MPE.IsBadgeRequired = 1 AND @ModifiedEndDate <= MPE.BadgeEndDate AND MPE.BadgeStartDate <= @OldEndDate

	END

	IF(@ProjectsExists = 0 AND @ModifiedStartDate IS NOT NULL)
	BEGIN

		SELECT @ProjectsExists = 1 
		FROM dbo.MilestonePersonEntry MPE 
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
		INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
		INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
		WHERE MP.PersonId = @PersonId AND P.ProjectStatusId IN (1,2,3,4) AND MPE.IsBadgeRequired = 1 AND @OldStartDate <= MPE.BadgeEndDate AND MPE.BadgeStartDate <= @ModifiedStartDate

	END

	SELECT @ProjectsExists

END
