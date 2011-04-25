--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Description:	List recruiter commission by the given recruit
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterCommissionListByRecruitId]
(
	@RecruitId   INT
)
AS
	SET NOCOUNT ON

	SELECT c.RecruiterId,
	       c.RecruitId,
	       c.HoursToCollect,
	       c.Amount
	  FROM dbo.RecruiterCommission AS c
	 WHERE c.RecruitId = @RecruitId
	ORDER BY c.HoursToCollect

