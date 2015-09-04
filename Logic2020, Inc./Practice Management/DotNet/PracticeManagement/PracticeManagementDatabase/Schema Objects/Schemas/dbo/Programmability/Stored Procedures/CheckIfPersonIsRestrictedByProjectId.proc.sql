CREATE PROCEDURE [dbo].[CheckIfPersonIsRestrictedByProjectId]
(
	@ProjectId		INT,
	@PersonId		INT,
	@ChargeDate		DATETIME
)
AS
BEGIN

	IF EXISTS(	SELECT 1
				FROM dbo.MilestonePersonEntry MPE 
				JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
				JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
				JOIN dbo.Project P ON P.ProjectId = M.ProjectId
				WHERE M.ProjectId = @ProjectId 
					  AND P.ClientId = 2 --Microsoft Account
					  AND MPE.IsBadgeRequired = 1 AND MPE.IsApproved = 0
					  AND MP.PersonId = @PersonId
					  AND (@ChargeDate BETWEEN MPE.BadgeStartDate AND MPE.BadgeEndDate)
			 )
	BEGIN
			SELECT CONVERT(BIT,1) Result 
	END
	ELSE
	BEGIN
			SELECT CONVERT(BIT,0) Result
	END

END
