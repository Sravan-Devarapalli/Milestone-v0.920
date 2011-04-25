CREATE PROCEDURE [dbo].[DefaultMilestoneSettingInsert]
(
	@ClientId		INT = NULL,
	@ProjectId		INT = NULL,
	@MilestoneId	INT = NULL,
	@LowerBound		INT = NULL,
	@UpperBound		INT = NULL
)
AS
	SET NOCOUNT ON

	IF NOT EXISTS (SELECT 1 FROM dbo.DefaultMilestoneSetting)
		INSERT INTO [dbo].[DefaultMileStoneSetting](ClientId,ProjectId,MileStoneId,ModifiedDate, LowerBound, UpperBound)
		SELECT @ClientId,@ProjectId,@MileStoneId,GETUTCDATE(),@LowerBound,@UpperBound
	ELSE
		UPDATE [dbo].[DefaultMileStoneSetting]
		SET ClientId		= ISNULL(@ClientId,ClientId),
			ProjectId		= ISNULL(@ProjectId,ProjectId),
			MilestoneId		= ISNULL(@MilestoneId,MilestoneId),
			ModifiedDate	= GETUTCDATE(),
			LowerBound		= ISNULL(@LowerBound,LowerBound),
			UpperBound		= ISNULL(@UpperBound,UpperBound)

