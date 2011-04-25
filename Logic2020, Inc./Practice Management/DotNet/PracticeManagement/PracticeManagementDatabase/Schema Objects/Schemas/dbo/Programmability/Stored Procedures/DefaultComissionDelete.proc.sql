--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Description:	Removes a person's default actual commision
-- =============================================
CREATE PROCEDURE [dbo].[DefaultComissionDelete]
(
	@PersonId   INT,
	@Type       INT
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @Today DATETIME
	SET @Today = dbo.[Today]()

	BEGIN TRAN

	-- Delete the commission was set today
	DELETE
	  FROM dbo.DefaultCommission
	 WHERE PersonId = @PersonId
	   AND [Type] = @Type
	   AND StartDate = @Today

	-- Expire previously set commission
	UPDATE dbo.DefaultCommission
	   SET EndDate = @Today
	 WHERE PersonId = @PersonId
	   AND [Type] = @Type
	   AND @Today >= StartDate
	   AND @Today < EndDate

	COMMIT TRAN

