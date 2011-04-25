-- =============================================
-- Author:		Alexey Zvekov
-- Create date: 7-1-1008
-- Updated by:	Anatoliy Lokshin
-- Update date:	11-18-2008
-- Description:	Sets person status and
-- implements inctivating person functionality
-- =============================================
CREATE PROCEDURE [dbo].[PersonSetStatus]
(
	@PersonID int,
	@PersonStatusId int
)
AS
	SET NOCOUNT ON

	IF @PersonStatusId <> 1	-- Inactive (Terminated or Projected)
	BEGIN

		DECLARE @today DATETIME
		SET @today = dbo.Today()

		IF EXISTS(SELECT 1
				FROM dbo.v_MilestonePersonLight AS mp
			   WHERE mp.PersonId = @PersonID
				 AND mp.EndDate > @today)
		BEGIN
			DECLARE @ErrorMessage NVARCHAR(2048)
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70006)
			RAISERROR (@ErrorMessage, 16, 1)
			RETURN
		END
	END
		
	UPDATE Person
		SET PersonStatusId = @PersonStatusId
		WHERE PersonId = @PersonID

