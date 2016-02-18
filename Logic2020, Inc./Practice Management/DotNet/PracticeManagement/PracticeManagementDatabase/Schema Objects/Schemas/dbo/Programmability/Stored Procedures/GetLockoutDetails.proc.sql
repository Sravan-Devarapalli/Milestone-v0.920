CREATE PROCEDURE [dbo].[GetLockoutDetails]
(
	@LockoutPageId		INT=NULL
)
AS
BEGIN

	SELECT	L.LockoutId,
			L.LockoutPageId,
			L.FunctionalityName,
			L.Lockout,
			L.LockoutDate
	FROM dbo.Lockout L
	JOIN dbo.LockoutPages LP ON LP.LockoutPageId = L.LockoutPageId
	WHERE @LockoutPageId IS NULL OR L.LockoutPageId = @LockoutPageId
	ORDER BY L.LockoutPageId

END

