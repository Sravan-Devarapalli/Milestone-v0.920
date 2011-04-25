--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Description:	Saves a person's default actual commisions
-- =============================================
CREATE PROCEDURE [dbo].[DefaultComissionSave]
(
	@PersonId           INT,
	@FractionOfMargin   DECIMAL(18,2),
	@Type               INT,
	@MarginTypeId       INT
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @Today DATETIME
	SET @Today = dbo.Today()

	BEGIN TRAN

	IF EXISTS(SELECT 1
	            FROM dbo.[DefaultCommission] AS c
	           WHERE c.PersonId = @PersonId
	             AND c.[Type] = @Type
	             AND c.StartDate = @Today)
	BEGIN
		-- The value was already changed today
		UPDATE dbo.[DefaultCommission]
		   SET FractionOfMargin = @FractionOfMargin,
		       MarginTypeId = @MarginTypeId
		 WHERE PersonId = @PersonId
		   AND [Type] = @Type
		   AND StartDate = @Today
	END
	ELSE
	BEGIN
		-- There is no value set today

		-- Expire old data
		UPDATE dbo.[DefaultCommission]
		   SET EndDate = @Today
		 WHERE PersonId = @PersonId
		   AND [Type] = @Type
		   AND @Today >= StartDate
		   AND @Today < EndDate

		-- Set new comission
		INSERT INTO dbo.[DefaultCommission]
		            (PersonId, StartDate, EndDate, FractionOfMargin, [type], MarginTypeId)
		     VALUES (@PersonId, @Today, '2029-12-31', @FractionOfMargin, @Type, @MarginTypeId)
	END

	COMMIT

