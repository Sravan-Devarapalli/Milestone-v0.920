-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Adds an item to the specified default recruiter commission.
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionItemInsert]
(
	@DefaultRecruiterCommissionHeaderId   INT,
	@HoursToCollect                       INT,
	@Amount                               DECIMAL(18,2)
)
AS
	SET NOCOUNT ON

	INSERT INTO dbo.DefaultRecruiterCommissionItem
	            (DefaultRecruiterCommissionHeaderId, HoursToCollect, Amount)
	     VALUES (@DefaultRecruiterCommissionHeaderId, @HoursToCollect, @Amount)

