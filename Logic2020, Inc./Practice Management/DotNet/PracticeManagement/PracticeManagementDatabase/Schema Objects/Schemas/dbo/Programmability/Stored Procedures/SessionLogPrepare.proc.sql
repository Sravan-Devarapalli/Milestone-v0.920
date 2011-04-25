


CREATE PROCEDURE [dbo].[SessionLogPrepare]
(
	@UserLogin     NVARCHAR(255)
)
AS
	SET NOCOUNT ON

	IF NOT EXISTS (SELECT 1 FROM dbo.SessionLogData WHERE SessionID = @@SPID)
	BEGIN
	INSERT INTO dbo.SessionLogData
				(SessionID, UserLogin, PersonID, LastName, FirstName)
	SELECT @@SPID, @UserLogin, MAX(p.PersonId), MAX(p.LastName), MAX(p.FirstName)
		FROM dbo.Person AS p
		WHERE p.Alias = @UserLogin
	END

