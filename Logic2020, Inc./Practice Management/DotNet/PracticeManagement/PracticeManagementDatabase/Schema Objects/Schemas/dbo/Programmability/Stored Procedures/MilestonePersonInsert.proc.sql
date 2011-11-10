-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-09-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-17-2008
-- Description:	Inserts persons-milestones association for the specified milestone and person.
-- =============================================
CREATE PROCEDURE [dbo].[MilestonePersonInsert]
(
	@MilestoneId         INT,
	@PersonId            INT,
	@MilestonePersonId   INT OUTPUT
)
AS
	SET NOCOUNT ON

	IF EXISTS(SELECT 1 FROM dbo.MilestonePerson WHERE MilestoneId = @MilestoneId AND PersonId = @PersonId)
	BEGIN
		
		SELECT TOP(1) @MilestonePersonId = MilestonePersonId 
		FROM dbo.MilestonePerson 
		WHERE MilestoneId = @MilestoneId AND PersonId = @PersonId

	END
	ELSE
	BEGIN
		INSERT INTO dbo.MilestonePerson
					(MilestoneId, PersonId)
			 VALUES (@MilestoneId, @PersonId)
		
		SET @MilestonePersonId = SCOPE_IDENTITY()
	END

