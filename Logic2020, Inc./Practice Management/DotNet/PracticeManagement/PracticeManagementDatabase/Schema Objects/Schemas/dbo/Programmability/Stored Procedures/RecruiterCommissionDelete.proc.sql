-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	12-01-2008
-- Description:	Deletes a recruiter commission for the given recruit
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterCommissionDelete]
(
	@RecruitId            INT,
	@RecruiterId          INT,
	@OLD_HoursToCollect   INT
)
AS
	SET NOCOUNT ON

	DELETE
	  FROM dbo.RecruiterCommission
	 WHERE RecruitId = @RecruitId
	   AND RecruiterId = @RecruiterId
	   AND HoursToCollect = @OLD_HoursToCollect

