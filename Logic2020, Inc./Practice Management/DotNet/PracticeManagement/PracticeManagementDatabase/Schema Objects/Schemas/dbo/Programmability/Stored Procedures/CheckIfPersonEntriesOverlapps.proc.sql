CREATE PROCEDURE [dbo].[CheckIfPersonEntriesOverlapps]
	(
		@MilestoneId	INT,
		@PersonId		INT,
		@StartDate		DATETIME,
		@EndDate		DATETIME
	)
AS
BEGIN
		DECLARE @ProjectId	INT,
				@NotValid	BIT = 0

		SELECT @ProjectId = ProjectId FROM dbo.Milestone WHERE MilestoneId = @MilestoneId

		IF EXISTS 
		(
			SELECT 1 
			FROM dbo.MilestonePersonEntry MPE
			JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId 
			WHERE MP.PersonId = @PersonId AND M.ProjectId = @ProjectId AND MP.MilestoneId <> @MilestoneId
			AND (@StartDate <= MPE.EndDate AND MPE.StartDate <= @EndDate)
		)
		BEGIN
			SET @NotValid = 1
		END

		SELECT @NotValid
END
