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

	IF EXISTS (SELECT 1
	             FROM dbo.RecruiterCommission AS c
	            WHERE c.RecruitId = @RecruitId
	              AND c.HoursToCollect = @OLD_HoursToCollect)
	BEGIN
		UPDATE dbo.RecruiterCommission
		   SET Amount = @Amount,
		       HoursToCollect = @HoursToCollect
		 WHERE RecruitId = @RecruitId
		   AND HoursToCollect = @OLD_HoursToCollect
	END
	ELSE
	BEGIN
		INSERT INTO dbo.RecruiterCommission
		            (RecruiterId, RecruitId, HoursToCollect, Amount)
		     VALUES (@RecruiterId, @RecruitId, @HoursToCollect, @Amount)
	END

