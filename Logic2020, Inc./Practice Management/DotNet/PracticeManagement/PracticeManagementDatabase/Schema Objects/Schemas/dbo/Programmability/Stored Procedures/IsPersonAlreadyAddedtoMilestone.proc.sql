CREATE PROCEDURE [dbo].[IsPersonAlreadyAddedtoMilestone]
(
	@MilestoneId         INT,
	@PersonId            INT
)
AS
	SET NOCOUNT ON

	IF EXISTS(SELECT 1 FROM dbo.MilestonePerson WHERE MilestoneId = @MilestoneId AND PersonId = @PersonId)
	BEGIN
		SELECT 1 AS Result
	END
	ELSE
	BEGIN
		SELECT 0 AS Result
	END

