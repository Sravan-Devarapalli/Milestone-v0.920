-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Adds an item to the specified default recruiter commission.
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionItemUpdate]
(
	@DefaultRecruiterCommissionHeaderId   INT,
	@HoursToCollect                       INT,
	@Amount                               DECIMAL(18,2)
)
AS
	SET NOCOUNT ON

	UPDATE dbo.DefaultRecruiterCommissionItem
	   SET Amount = @Amount
	 WHERE DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId
	   AND HoursToCollect = @HoursToCollect

