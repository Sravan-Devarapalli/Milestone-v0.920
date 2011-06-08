-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-29-2008
-- Description:	Saves a recruiter commission for the given recruit
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterCommissionSave]
(
	@RecruiterId          INT,
	@RecruitId            INT,
	@HoursToCollect       INT,
	@Amount               DECIMAL(18,2),
	@OLD_HoursToCollect   INT
)
AS
	SET NOCOUNT ON

	IF(@Amount IS NOT NULL AND
		EXISTS (SELECT 1 FROM dbo.RecruiterCommission  WITH(NOLOCK)
				WHERE RecruitId = @RecruitId AND Amount IS NULL)
	)
	BEGIN
		DELETE FROM dbo.RecruiterCommission  
        WHERE  RecruitId = @RecruitId AND Amount IS NULL
	END

	
	IF EXISTS (SELECT 1
					FROM dbo.RecruiterCommission AS c
				WHERE c.RecruitId = @RecruitId
					AND c.HoursToCollect = @OLD_HoursToCollect
					AND c.RecruiterId = @RecruiterId)
	BEGIN

		UPDATE dbo.RecruiterCommission
			SET Amount = @Amount,
				HoursToCollect = @HoursToCollect
			WHERE RecruitId = @RecruitId
			AND HoursToCollect = @OLD_HoursToCollect   
	END
	ELSE
	BEGIN
		IF @HoursToCollect <> 0 OR
		NOT EXISTS (SELECT 1 FROM dbo.RecruiterCommission WHERE RecruitId = @RecruitId 
						AND RecruiterId = @RecruiterId AND HoursToCollect = 0)
		BEGIN
			INSERT INTO dbo.RecruiterCommission
					(RecruiterId, RecruitId, HoursToCollect, Amount)
				VALUES (@RecruiterId, @RecruitId, @HoursToCollect, @Amount)
		END
		ELSE
		BEGIN
				
			UPDATE dbo.RecruiterCommission
				SET Amount = @Amount 
				WHERE RecruitId = @RecruitId
				AND RecruiterId = @RecruiterId
				AND HoursToCollect = 0

		END
END
